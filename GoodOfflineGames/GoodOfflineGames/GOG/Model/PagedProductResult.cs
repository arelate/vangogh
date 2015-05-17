using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    public class ProductsResult
    {
        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }

        public ProductsResult()
        {
            Products = new List<Product>();
        }

        public static async Task<ProductsResult> RequestUpdated(
            ProductsResult existing,
            string uri,
            Dictionary<string, string> queryParameters,
            IConsoleController consoleController,
            string message)
        {
            string pageQueryParameter = "page";
            int currentPageIndex = 1;
            string currentPageJson = string.Empty;
            ProductsResult currentPage = null;
            int totalNewProducts = 0;

            if (!queryParameters.Keys.Contains(pageQueryParameter))
            {
                queryParameters.Add(pageQueryParameter, currentPageIndex.ToString());
            }

            consoleController.Write(message);

            do
            {
                queryParameters[pageQueryParameter] = currentPageIndex.ToString();
                consoleController.Write("{0}..", currentPageIndex);

                currentPageJson = await Network.Request(uri, queryParameters);
                currentPage = JSON.Parse<ProductsResult>(currentPageJson);

                var newProducts = SelectNewProducts(currentPage, existing);

                if (newProducts != null &&
                    newProducts.Count > 0)
                {
                    totalNewProducts += newProducts.Count;
                    existing.Products.AddRange(newProducts);
                }
                else
                {
                    // ... current page didn't produce any new products
                    break;
                }

            }
            while (++currentPageIndex <= currentPage.TotalPages);

            consoleController.WriteLine("Got {0} new products.", totalNewProducts);

            return existing;
        }


        public Product GetProductById(long id)
        {
            if (Products == null)
            {
                return null;
            }

            foreach (var product in Products)
            {
                if (product.Id == id)
                {
                    return product;
                }
            }

            return null;
        }

        public bool Contains(Product product)
        {
            var existingProduct = GetProductById(product.Id);
            return existingProduct != null;
        }

        private static List<Product> SelectNewProducts(ProductsResult currentPage, ProductsResult existing)
        {
            if (currentPage == null ||
                currentPage.Products == null)
            {
                return null;
            }

            List<Product> newProducts = new List<Product>();

            foreach (Product product in currentPage.Products)
            {
                if (!existing.Contains(product))
                {
                    newProducts.Add(product);
                }
            }

            return newProducts;
        }

        public static void MarkAllOwned(ProductsResult result)
        {
            if (result != null &&
                result.Products != null)
            {
                foreach (var product in result.Products)
                {
                    product.Owned = true;
                }
            }
        }

        public static int MergeOwned(ProductsResult existing, ProductsResult owned)
        {
            if (existing == null ||
                owned == null)
            {
                return 0;
            }

            var updated = 0;

            foreach (var product in owned.Products)
            {
                if (product == null)
                {
                    continue;
                }

                var game = existing.GetProductById(product.Id);

                if (game != null)
                {
                    game.Owned = true;
                    updated++;
                }
                else
                {
                    existing.Products.Add(product);
                }
            }

            return updated;
        }
    }
}
