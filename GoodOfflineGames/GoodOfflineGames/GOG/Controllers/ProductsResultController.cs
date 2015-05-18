using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GOG
{
    public class ProductsResultController
    {
        protected ProductsResult productsResult;

        public ProductsResultController(ProductsResult productsResult)
        {
            this.productsResult = productsResult;
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

                var json = await NetworkController.RequestString(uri, queryParameters);
                currentPage = JSONController.Parse<ProductsResult>(json);

                var newProducts =
                    currentPage.Products.Where(
                        cp =>
                        !existing.Products.Any(
                            ep =>
                            ep.Id == cp.Id));

                if (newProducts != null &&
                    newProducts.Count() > 0)
                {
                    totalNewProducts += newProducts.Count();
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

        public void UpdateOwned()
        {
            productsResult.Products.Select(p => p.Owned = true);
        }

        public void MergeOwned(ProductsResult owned)
        {
            if (owned == null) return;

            owned.Products.ForEach(op =>
            {
                var game = productsResult.Products.Find(p => p.Id == op.Id);

                if (game != null) game.Owned = true;
                else productsResult.Products.Add(op);

            });
        }
    }
}

