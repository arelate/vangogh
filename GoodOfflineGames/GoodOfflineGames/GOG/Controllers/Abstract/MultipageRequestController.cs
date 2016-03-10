using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;
using GOG.Interfaces;

namespace GOG.Controllers
{
    class MultipageRequestController : IRequestDelegate<Product>
    {
        private IGetStringDelegate getStringDelegate;
        private ISerializationController<string> stringifyController;
        private IPostUpdateDelegate postUpdateDelegate;

        private IFilterDelegate<Product> filterDelegate;
        public IWriteDelegate MessageWriteDelegate { get; set; } = null;

        public MultipageRequestController(
            IGetStringDelegate getStringDelegate,
            ISerializationController<string> stringifyController,
            IPostUpdateDelegate postUpdateDelegate = null,
            IFilterDelegate<Product> filterDelegate = null)
        {
            this.stringifyController = stringifyController;
            this.getStringDelegate = getStringDelegate;
            this.postUpdateDelegate = postUpdateDelegate;
            this.filterDelegate = filterDelegate;
        }

        public async Task<IList<Product>> Request(
            string uri, 
            IDictionary<string, string> parameters, 
            IList<Product> filter = null)
        {
            var currentPage = 1;
            ProductsResult current;
            var products = new List<Product>();

            do
            {
                if (postUpdateDelegate != null)
                    postUpdateDelegate.PostUpdate();

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

            var json = await getStringDelegate.GetString(uri, parameters);
            return stringifyController.Deserialize<ProductsResult>(json);
        }

    }
}
