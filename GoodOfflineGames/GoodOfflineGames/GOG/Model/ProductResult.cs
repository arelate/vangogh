using System.Collections.Generic;
using System.Linq;
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

        public void MarkAllAsOwned()
        {
            Products.Select(p => p.Owned = true);
        }

        public void MergeOwned(ProductsResult owned)
        {
            if (owned == null) return;

            owned.Products.ForEach(op =>
            {
                var game = Products.Find(p => p.Id == op.Id);

                if (game != null) game.Owned = true;
                else Products.Add(op);

            });
        }
    }
}
