using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GOG
{
    public class ProductDataController: ProductsResultController
    {
        public ProductDataController(ProductsResult productsResult) : base(productsResult)
        {
            // ... 
        }

        public async Task UpdateProductData(IConsoleController consoleController)
        {
            consoleController.Write("Updating product data for GOG.com products...");

            foreach (Product product in productsResult.Products)
            {
                //if (product.ProductData != null) continue;
                if (string.IsNullOrEmpty(product.Url)) continue;

                consoleController.Write(".");

                var gamePageUri = Urls.HttpRoot + product.Url;

                var gogData = await GogDataController.RequestData<GOGData>(gamePageUri);
                product.ProductData = gogData.ProductData;
            }

            consoleController.WriteLine("DONE.");
        }
    }
}
