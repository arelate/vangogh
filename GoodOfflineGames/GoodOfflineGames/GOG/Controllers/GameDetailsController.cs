using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;
using GOG.SharedModels;

namespace GOG.Controllers
{
    class GameDetailsController: ProductsResultController
    {
        public GameDetailsController(
            ProductsResult productsResult,
            IStringDataRequestController stringDataRequestController,
            ISerializationController serializationController) : 
            base(productsResult, stringDataRequestController, serializationController)
        {
            // ... 
        }

        public async Task UpdateGameDetails(IConsoleController consoleController)
        {
            consoleController.Write("Updating game details for owned products...");

            foreach (Product product in productsResult.Products.FindAll(p => p.Owned))
            {
                // skip games that already have game details and have no updates
                if (product.GameDetails != null &&
                    product.Updates == 0) continue;

                // skip DLCs as they won't have separate game details
                if (product.ProductData != null &&
                    product.ProductData.RequiredProducts != null &&
                    product.ProductData.RequiredProducts.Count > 0) continue;

                consoleController.Write(".");

                var gameDetailsUri = string.Format(Urls.AccountGameDetailsTemplate, product.Id);
                var gameDetails = await stringDataRequestController.RequestData<GameDetails>(gameDetailsUri);

                product.GameDetails = gameDetails;
            };

            consoleController.WriteLine("DONE.");
        }
    }
}
