using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    public class Games
    {
        private IConsoleController consoleController;

        public Games(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public async Task<GamesResult> GetGames()
        {
            return await PagedResults<GamesResult>.Request(
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                "Getting all games available on GOG.com...");
        }

        public int MergeAccountGames(GamesResult games, PagedProductsResult accountResult)
        {
            if (games == null ||
                accountResult == null)
            {
                return 0;
            }

            var updated = 0;

            foreach (var product in accountResult.Products)
            {
                if (product == null)
                {
                    continue;
                }

                var game = games.GetProductById(product.Id);

                if (game != null)
                {
                    game.Owned = true;
                    updated++;
                }
                else
                {
                    games.Products.Add(product);
                }
            }

            return updated;
        }
    }
}
