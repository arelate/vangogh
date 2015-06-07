using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using GOG.Interfaces;
using GOG.Models;

namespace GOG.Controllers
{
    public class ProductsResultController : IDisposable
    {
        public ProductsResult ProductsResult { get; set; }
        protected IStringGetController stringGetController;
        private ISerializationController serializationController;
        private IConsoleController consoleController;

        public ProductsResultController()
        {
            ProductsResult = new ProductsResult();
        }

        public ProductsResultController(List<Product> products): this()
        {
            this.ProductsResult = new ProductsResult(products);
        }

        public ProductsResultController(ProductsResult productsResult): this()
        {
            if (productsResult == null)
            {
                productsResult = new ProductsResult();
            }

            ProductsResult = productsResult;
        }

        public ProductsResultController(
            IStringGetController stringGetController,
            ISerializationController serializationController,
            IConsoleController consoleController,
            ProductsResult productsResult = null) :
            this(productsResult)
        {
            this.stringGetController = stringGetController;
            this.serializationController = serializationController;
            this.consoleController = consoleController;
        }

        private async Task<ProductsResult> RequestPage(string uri,
            IDictionary<string, string> parameters,
            int currentPage)
        {
            string pageQueryParameter = "page";

            if (!parameters.Keys.Contains(pageQueryParameter))
            {
                parameters.Add(pageQueryParameter, currentPage.ToString());
            }

            parameters[pageQueryParameter] = currentPage.ToString();

            var json = await stringGetController.GetString(uri, parameters);
            return serializationController.Parse<ProductsResult>(json);
        }

        private IEnumerable<Product> FilterNewProducts(ProductsResult current, ProductsResult existing)
        {
            foreach (Product maybeNew in current.Products)
            {
                var existingProduct = existing.Products.Find(p => p.Id == maybeNew.Id);
                if (existingProduct == null)
                {
                    yield return maybeNew;
                }
            }
        }

        private int MergeNewProducts(ProductsResult existing, IEnumerable<Product> newProducts)
        {
            var newProductsCount = 0;

            if (newProducts != null)
            {
                newProductsCount = newProducts.Count();
            }

            if (newProductsCount > 0)
            {
                // insert new games at the start
                existing.Products.InsertRange(0, newProducts);
            }

            return newProductsCount;
        }

        private async Task<List<Product>> RequestPagedResult(
            string uri,
            IDictionary<string, string> parameters,
            ProductsResult existing,
            bool breakOnExistingPageResult = true,
            string message = null)
        {
            consoleController.Write(message);
            //var updatedProducts = new ProductsResult(existing); 
            var allNewProducts = new List<Product>();

            int currentPage = 1;
            ProductsResult current;

            int totalNewProducts = 0;

            do
            {
                consoleController.Write(".");

                current = await RequestPage(uri, parameters, currentPage);

                var newProducts = (existing != null) ?
                    FilterNewProducts(current, existing) :
                    current.Products;
                var newProductsCount = MergeNewProducts(ProductsResult, newProducts);
                allNewProducts.AddRange(newProducts);

                totalNewProducts += newProductsCount;

                if (newProductsCount == 0 &&
                    breakOnExistingPageResult)
                {
                    break;
                }
            }
            while (++currentPage <= current.TotalPages);

            consoleController.WriteLine("Got {0} products.", totalNewProducts);

            return allNewProducts;
        }


        public async Task<List<Product>> GetUpdated(
            string uri,
            IDictionary<string, string> parameters,
            ProductsResult existing,
            string message = null)
        {
            var updated = await RequestPagedResult(uri, parameters, existing, true, message);

            ProductsResult = new ProductsResult(existing);
            ProductsResult.Products.InsertRange(0, updated);

            return updated;
        }

        public async Task<List<Product>> GetAll(
            string uri,
            Dictionary<string, string> parameters,
            string message = null)
        {
            return await RequestPagedResult(uri, parameters, null, false, message);
        }

        public void SetAllAsOwned(List<Product> products)
        {
            foreach (Product p in products)
            {
                p.Owned = true;
            }
        }

        private void MergeDLCs(GameDetails dlc)
        {
            var dlcTitle = dlc.Title.Replace("DLC: ", string.Empty);

            var game = ProductsResult.Products.Find(p => p.Title == dlcTitle);
            if (game != null) game.Owned = true;
            else
            {
                // ah... I see you have discovered a hidden item
                // that should be ok - not all DLCs are available as product 
                // (e.g. special preorder bonuses and kickstarter items)
            }

            if (dlc.DLCs != null &&
                dlc.DLCs.Count > 0)
            {
                foreach (GameDetails childDlc in dlc.DLCs)
                {
                    MergeDLCs(childDlc);
                }
            }
        }

        public void MergeOwned(IEnumerable<Product> owned)
        {
            if (owned == null) return;

            foreach (var op in owned)
            {
                var game = ProductsResult.Products.Find(p => p.Id == op.Id);

                if (game != null) game.Owned = true;
                else ProductsResult.Products.Add(op);

                // also merge DLC as owned
                if (op.GameDetails != null &&
                    op.GameDetails.DLCs != null &&
                    op.GameDetails.DLCs.Count > 0)
                {
                    foreach (GameDetails dlc in op.GameDetails.DLCs)
                    {
                        MergeDLCs(dlc);
                    }
                }
            }
        }

        public void MergeUpdated(List<Product> updated)
        {
            if (updated == null) return;

            foreach (Product op in updated)
            {
                var game = ProductsResult.Products.Find(p => p.Id == op.Id);
                if (game != null) game.Updates = 1;
                else throw new InvalidOperationException("Games that are not owned cannot be marked updated.");
            }
        }

        public void ResetUpdated()
        {
            ProductsResult.Products.ForEach(p => p.Updates = 0);
        }

        public void MergeWishlisted(ProductsResult wishlisted)
        {
            wishlisted.Products.ForEach(wp =>
            {
                var wishlistedProduct = ProductsResult.Products.Find(p => p.Id == wp.Id);
                wishlistedProduct.Wishlisted = true;
            });
        }

        public async Task UpdateProductDetails(IProductDetailsProvider<Product> detailsProvider)
        {
            consoleController.Write(detailsProvider.Message);
            int totalProductsUpdated = 0;

            foreach (Product product in ProductsResult.Products)
            {
                if (detailsProvider.SkipCondition(product))
                {
                    continue;
                }

                consoleController.Write(".");

                var requestUri = string.Format(detailsProvider.RequestTemplate,
                    detailsProvider.GetRequestDetails(product));
                var detailsString = await detailsProvider.StringGetController.GetString(requestUri);

                detailsProvider.SetDetails(product, detailsString);
                totalProductsUpdated++;
            }

            consoleController.WriteLine("Updated details for {0} products.", totalProductsUpdated);
        }

        private IEnumerable<Product> Reduce(Predicate<Product> condition)
        {
            foreach (Product p in ProductsResult.Products)
            {
                if (condition(p)) yield return p;
            }
        }

        public IEnumerable<Product> GetOwned()
        {
            return Reduce(p => p.Owned);
        }

        public IEnumerable<Product> GetUpdated()
        {
            return Reduce(p => p.Updates > 0);
        }

        public IEnumerable<Product> GetByName(IEnumerable<string> names)
        {
            return Reduce(p => names.Contains(p.Title));
        }

        public void Dispose()
        {
            ProductsResult = null;
        }
    }
}

