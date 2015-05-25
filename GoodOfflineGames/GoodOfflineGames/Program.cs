namespace GOG
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleController = new ConsoleController();
            var ioController = new IOController();
            var storage = new StorageController(ioController);
            var jsonController = new JSONController();
            var uriController = new UriController();
            var networkController = new NetworkController(uriController);
            var settingsController = new SettingsController(
                ioController, 
                jsonController, 
                consoleController);
            var authenticationController = new AuthenticationController(
                uriController, 
                networkController,
                consoleController);

            var productsResultController = new ProductsResultController(
                null,
                networkController,
                jsonController);

            var jsonDataPrefix = "var data = ";
            var filename = "data.js";
            ProductsResult storedGames = null;

            // Try to load stored games data and use it to update games rather than download again
            try
            {
                var storedGamesJson = storage.Pull(filename)
                    .Result
                    .Replace(jsonDataPrefix, string.Empty);
                storedGames = jsonController.Parse<ProductsResult>(storedGamesJson);
            }
            catch
            {
                // file not found or couldn't be read
                storedGames = new ProductsResult();
            }

            var settings = settingsController.LoadSettings().Result;

            if (!authenticationController.AuthorizeOnSite(settings).Result)
            {
                consoleController.WriteLine("Press ENTER to exit...");
                consoleController.ReadLine();
                return;
            }

            // get all available games from gog.com/games

            var gamesResult = productsResultController.RequestNew(storedGames,
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                "Getting all games available on GOG.com...").Result;

            // get all owned games from gog.com/account

            // filter owned products, so that we can check if account has got new owned games,
            // and not just new games, also this is critical as we mark games as owned later

            var storedOwned = new ProductsResult();
            storedOwned.Products = storedGames.Products.FindAll(p => p.Owned);

            // get all games owned by user

            var ownedResult = productsResultController.RequestNew(storedOwned,
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                consoleController,
                "Getting account games...").Result;

            // separately get all updated products

            QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            var updatedResult = productsResultController.RequestNew(null,
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                consoleController,
                "Getting updated games...").Result;

            var ownedResultController = new ProductsResultController(ownedResult);

            // mark all new owned games as owned
            //ownedResultController.ResetUpdated();

            // reset updates between data source modifications
            ownedResultController.UpdateOwned();

            // merge owned games into all available games 

            var gamesResultController = new ProductsResultController(gamesResult);

            gamesResultController.MergeOwned(ownedResult);

            // merge updated games into owned games
            gamesResultController.MergeUpdated(updatedResult);

            // update details for individual games
            // TODO: create single loop with parameters to update based on templates and predicates

            // update product data from game pages

            var productDataController = new ProductDataController(
                gamesResult, 
                networkController, 
                jsonController);
            productDataController.UpdateProductData(consoleController).Wait();

            // update game details for all owned games

            var gameDetailsController = new GameDetailsController(
                gamesResult, 
                networkController, 
                jsonController);
            gameDetailsController.UpdateGameDetails(consoleController).Wait();

            // update wishlisted games
            var wishlistController = new WishlistController(
                gamesResult, 
                networkController, 
                jsonController);
            wishlistController.UpdateWishlisted(consoleController).Wait();

            // serialize and save on disk

            var gamesResultJson = "var data = " + jsonController.Stringify(gamesResult);
            storage.Put(filename, gamesResultJson).Wait();

            // download new images 

            var images = new ImagesController(networkController, ioController, consoleController);
            images.Update(gamesResult).Wait();

            // nothing left to do here

            consoleController.WriteLine("All done. Press ENTER to quit...");
            consoleController.ReadLine();
        }
    }
}
