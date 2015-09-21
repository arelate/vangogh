using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;
using GOG.Interfaces;

namespace GOG.Controllers
{
    class MultipageRequestController : IMultipageRequestController<Product, long>
    {
        private IStringGetController stringGetController;
        private IStringifyController stringifyController;
        private IConsoleController consoleController;

        private IFilterDelegate<Product, long> filterDelegate;
        public IWriteController MessageWriteDelegate { get; set; } = null;

        public MultipageRequestController(
            IStringGetController stringGetController,
            IStringifyController stringifyController,
            IConsoleController consoleController = null,
            IFilterDelegate<Product, long> filterDelegate = null)
        {
            this.stringifyController = stringifyController;
            this.stringGetController = stringGetController;
            this.filterDelegate = filterDelegate;
        }

        public async Task<IList<Product>> Request(
            string uri, 
            IDictionary<string, string> parameters, 
            IList<long> filter = null)
        {
            var currentPage = 1;
            ProductsResult current;
            var products = new List<Product>();

            do
            {
                if (consoleController != null)
                    consoleController.Write(".");

                current = await RequestPage(uri, parameters, currentPage);

                if (filter != null && filterDelegate != null)
                {
                    var filteredProducts = filterDelegate.Filter(current.Products, filter);
                    products.AddRange(filteredProducts);

                    if (filteredProducts.Count() == 0) break;

                }
                else
                {
                    products.AddRange(current.Products);
                }
            }
            while (++currentPage <= current.TotalPages);

            if (consoleController != null)
                consoleController.WriteLine("Got {0} products.", products.Count);

            return products;
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
            return stringifyController.Parse<ProductsResult>(json);
        }

    }
}
