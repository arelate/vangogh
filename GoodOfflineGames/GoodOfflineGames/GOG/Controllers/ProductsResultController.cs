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
    public class ProductsResultController
    {
        protected ProductsResult productsResult;
        protected IStringRequestController stringRequestController;
        private ISerializationController serializationController;
        private IConsoleController consoleController;

        public ProductsResultController(ProductsResult productsResult)
        {
            if (productsResult == null)
            {
                productsResult = new ProductsResult();
            }

            this.productsResult = productsResult;
        }

        public ProductsResultController(
            IStringRequestController stringRequestController,
            ISerializationController serializationController,
            IConsoleController consoleController,
            ProductsResult productsResult = null) :
            this(productsResult)
        {
            this.stringRequestController = stringRequestController;
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

            var json = await stringRequestController.RequestString(uri, parameters);
            return serializationController.Parse<ProductsResult>(json);
        }

        private IEnumerable<Product> FilterNewProducts(ProductsResult existing, ProductsResult current)
        {
            var newProducts =
                current.Products.Where(
                    cp =>
                    !existing.Products.Any(ep => ep.Id == cp.Id));
            return newProducts;
        }

        private int MergeNewProducts(ProductsResult existing, IEnumerable<Product> newProducts)
        {
            var newProductsCount = 0;

            if (newProducts != null) {
                newProductsCount = newProducts.Count();
            }

            if (newProductsCount > 0)
            {
                // insert new games at the start
                existing.Products.InsertRange(0, newProducts);
            }

            return newProductsCount;
        }

        private async Task<ProductsResult> RequestPagedResult(
            string uri,
            IDictionary<string, string> parameters,
            ProductsResult existing,
            bool breakOnExistingPageResult = true,
            string message = null)
        {
            consoleController.Write(message);

            int currentPage = 1;
            ProductsResult current;
            if (existing == null)
            {
                existing = new ProductsResult();
            }

            int totalNewProducts = 0;

            do
            {
                consoleController.Write(".");

                current = await RequestPage(uri, parameters, currentPage);

                var newProducts = FilterNewProducts(existing, current);
                var newProductsCount = MergeNewProducts(existing, newProducts);

                totalNewProducts += newProductsCount;

                if (newProductsCount == 0 &&
                    breakOnExistingPageResult)
                {
                    break;
                }
            }
            while (++currentPage <= current.TotalPages);

            consoleController.WriteLine("Got {0} products.", totalNewProducts);

            return existing;
        }


        public async Task<ProductsResult> UpdateExisting(
            string uri,
            IDictionary<string, string> parameters,
            ProductsResult existing,
            string message = null)
        {
            return await RequestPagedResult(uri, parameters, existing, true, message);
        }

        public async Task<ProductsResult> GetAll(
            string uri,
            Dictionary<string, string> parameters,
            string message = null)
        {
            return await RequestPagedResult(uri, parameters, null, false, message);
        }

        public void SetAllAsOwned()
        {
            foreach (Product p in productsResult.Products)
            {
                p.Owned = true;
            }
        }

        private void MergeDLCs(GameDetails dlc)
        {
            var dlcTitle = dlc.Title.Replace("DLC: ", string.Empty);

            var game = productsResult.Products.Find(p => p.Title == dlcTitle);
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

        public void MergeOwned(ProductsResult owned)
        {
            if (owned == null) return;

            owned.Products.ForEach(op =>
            {
                var game = productsResult.Products.Find(p => p.Id == op.Id);

                if (game != null) game.Owned = true;
                else productsResult.Products.Add(op);

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

            });
        }

        public void MergeUpdated(ProductsResult updated)
        {
            if (updated == null) return;

            updated.Products.ForEach(op =>
            {
                var game = productsResult.Products.Find(p => p.Id == op.Id);
                if (game != null) game.Updates = 1;
                else throw new InvalidOperationException("Games that are not owned cannot be marked updated.");
            });
        }

        public void ResetUpdated()
        {
            this.productsResult.Products.ForEach(p => p.Updates = 0);
        }

        public void MergeWishlisted(ProductsResult wishlisted)
        {
            wishlisted.Products.ForEach(wp =>
            {
                var wishlistedProduct = productsResult.Products.Find(p => p.Id == wp.Id);
                wishlistedProduct.Wishlisted = true;
            });
        }

        public async Task UpdateProductDetails(IProductDetailsProvider<Product> detailsProvider)
        {
            consoleController.Write(detailsProvider.Message);
            int totalProductsUpdated = 0;

            foreach (Product product in productsResult.Products)
            {
                if (detailsProvider.SkipCondition(product))
                {
                    continue;
                }

                consoleController.Write(".");

                var requestUri = string.Format(detailsProvider.RequestTemplate,
                    detailsProvider.GetRequestDetails(product));
                var detailsString = await detailsProvider.StringRequestController.RequestString(requestUri);

                detailsProvider.SetDetails(product, detailsString);
                totalProductsUpdated++;
            }

            consoleController.WriteLine("Updated details for {0} products.", totalProductsUpdated);
        }

    }
}

