using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;
using GOG.SharedModels;

namespace GOG.Controllers
{
    public class ProductDataController: ProductsResultController
    {
        public ProductDataController(ProductsResult productsResult,
            IStringDataRequestController stringDataRequestController,
            ISerializationController serializationController) :
            base(productsResult, stringDataRequestController, serializationController)
        {
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

                var gogData = await stringDataRequestController.RequestData<GOGData>(gamePageUri);
                product.ProductData = gogData.ProductData;
            }

            consoleController.WriteLine("DONE.");
        }
    }
}
