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
                // skip games that already have game details
                // TODO: make sure we have the right games marked as updated
                if (product.GameDetails != null)
                    continue;

                consoleController.Write(".");

                var gameDetailsUri = string.Format(Urls.AccountGameDetailsTemplate, product.Id);
                var gameDetails = await NetworkController.RequestData<GameDetails>(gameDetailsUri);

                product.GameDetails = gameDetails;
            };

            consoleController.WriteLine("DONE.");
        }
    }
}
