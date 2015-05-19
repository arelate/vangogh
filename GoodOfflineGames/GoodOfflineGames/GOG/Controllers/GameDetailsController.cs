using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    class GameDetailsController: ProductsResultController
    {
        public GameDetailsController(ProductsResult productsResult) : base(productsResult)
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

                consoleController.Write(".");

                var gameDetailsUri = string.Format(Urls.AccountGameDetailsTemplate, product.Id);
                var gameDetails = await NetworkController.RequestData<GameDetails>(gameDetailsUri);

                product.GameDetails = gameDetails;

                // mark the game as not updated to avoid the need to update details in the future
                product.Updates = 0;
            };

            consoleController.WriteLine("DONE.");
        }
    }
}
