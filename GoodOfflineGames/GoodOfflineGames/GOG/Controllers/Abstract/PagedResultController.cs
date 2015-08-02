using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Models;
using GOG.Interfaces;

namespace GOG.Controllers
{
    class PagedResultController : IPagedResultController
    {
        private IStringGetController stringGetController;
        private IStringifyController stringifyController;

        public IPagedResultFilterDelegate FilterDelegate { get; set; } = null;
        public IWriteController MessageWriteDelegate { get; set; } = null;

        public PagedResultController(
            IStringGetController stringGetController,
            IStringifyController stringifyController)
        {

            this.stringifyController = stringifyController;
            this.stringGetController = stringGetController;
        }

        public async Task<IList<Product>> GetAll(string uri, IDictionary<string, string> parameters)
        {
            var currentPage = 1;
            ProductsResult current;
            var products = new List<Product>();

            do
            {
                if (MessageWriteDelegate != null)
                {
                    MessageWriteDelegate.Write(".");
                }

                current = await GetOne(uri, parameters, currentPage);

                if (FilterDelegate != null)
                {
                    var filteredProducts = FilterDelegate.Filter(current.Products);
                    products.AddRange(filteredProducts);

                    if (filteredProducts.Count() == 0) break;

                }
                else
                {
                    products.AddRange(current.Products);
                }
            }
            while (++currentPage <= current.TotalPages);

            if (MessageWriteDelegate != null)
            {
                MessageWriteDelegate.WriteLine("Got {0} products.", products.Count);
            }

            return products;
        }

        public async Task<ProductsResult> GetOne(string uri,
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
