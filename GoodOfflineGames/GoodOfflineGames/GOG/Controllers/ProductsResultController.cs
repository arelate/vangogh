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

        public static async Task<ProductsResult> RequestNew(
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

            if (existing == null)
            {
                existing = new ProductsResult();
            }

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
                    // insert new games at the start
                    existing.Products.InsertRange(0, newProducts);
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

        private void MergeDLCs(GameDetails dlc)
        {
            var dlcTitle = dlc.Title.Replace("DLC: ", string.Empty);

            var game = productsResult.Products.Find(p => p.Title == dlcTitle);
            if (game != null) game.Owned = true;
            else
            {
                Console.WriteLine("Didn't find product for DLC {0}", dlcTitle);
            }
            //else throw new InvalidOperationException("Couldn't find DLC product by title.");

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

    }
}

