using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    public static class PagedResults<T> where T: PagedProductsResult
    {
        public static async Task<T> Request(
            string uri,
            Dictionary<string, string> queryParameters,
            IConsoleController consoleController,
            string message)
        {
            string pageQueryParameter = "page";
            int currentPage = 1;
            string currentPageJson = string.Empty;
            T currentPageResults = null;
            T results = null;

            if (!queryParameters.Keys.Contains(pageQueryParameter))
            {
                queryParameters.Add(pageQueryParameter, currentPage.ToString());
            }

            consoleController.Write(message);

            do
            {
                queryParameters[pageQueryParameter] = currentPage.ToString();
                consoleController.Write("{0}..", currentPage);

                currentPageJson = await Network.Request(uri, queryParameters);
                currentPageResults = JSON.Parse<T>(currentPageJson);

                if (results == null)
                {
                    results = currentPageResults;
                } else
                {
                    results.Products.AddRange(currentPageResults.Products);
                }

            } while (++currentPage <= currentPageResults.TotalPages);

            consoleController.WriteLine("Got {0} products.", results.Products.Count);

            return results;
        }
    }
}
