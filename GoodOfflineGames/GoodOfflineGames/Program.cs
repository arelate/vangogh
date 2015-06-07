using System.Collections.Generic;

using GOG.SharedControllers;
using GOG.Controllers;
using GOG.Models;
using GOG.SharedModels;
using GOG.Providers;

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

            var productsResultController = new ProductsResultController(
                networkController,
                jsonController,
                consoleController);

            // get all owned games from gog.com/account

            // filter owned products, so that we can check if account has got new owned games,
            // and not just new games, also this is critical as we mark games as owned later

            var storedOwned = new ProductsResult();
            storedOwned.Products = storedGames.Products.FindAll(p => p.Owned);

            // get all games owned by user
            var updatedOwned = productsResultController.GetUpdated(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                storedOwned,
                "Getting new account games...").Result;

            // get all available games from gog.com/games
            productsResultController.GetUpdated(
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                storedGames,
                "Getting new games available on GOG.com...").Wait();

            // separately get all updated products
            QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            var updatedResult = productsResultController.GetAll(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                "Getting updated games...").Result;

            // mark all new owned games as owned
            productsResultController.SetAllAsOwned(updatedOwned);

            // merge owned games into all available games 
            productsResultController.MergeOwned(updatedOwned);

            // merge updated games into owned games
            productsResultController.MergeUpdated(updatedResult);

            // update details for individual games
            var gogDataController = new GOGDataController(networkController);

            // update product data from game pages
            var productDataProvider = new ProductDataProvider(gogDataController, jsonController);
            productsResultController.UpdateProductDetails(productDataProvider).Wait();

            // update game details for all owned games
            var gameDetailsProvider = new GameDetailsProvider(networkController, jsonController);
            productsResultController.UpdateProductDetails(gameDetailsProvider).Wait();

            using (var wishlistController = new WishlistController(gogDataController, jsonController))
            {
                // update wishlisted games
                var wishlistResult = wishlistController.RequestWishlisted(consoleController).Result;

                productsResultController.MergeWishlisted(wishlistResult);
            }

            // get product files for updated and new files
            var updatedProducts = productsResultController.GetUpdated();

            var downloadProducts = new List<Product>(updatedProducts);

            // when we got updated owned products they didn't have download links,
            // since then we've updated all product details and productsResultController should have
            // all the details we need - so for every newly obtained product we need to find match 
            // in current productsReusltController to use for updating files
            foreach (var uo in updatedOwned)
            {
                var updatedOwnedWithDetails = productsResultController.ProductsResult.Products.Find(p => p.Id == uo.Id);
                if (updatedOwnedWithDetails != null)
                {
                    downloadProducts.Add(updatedOwnedWithDetails);
                }
            }

            // also if we've passed products for manual update through settings.json
            // add them for update as well

            if (settings.ManualUpdate != null)
            {
                var manualUpdate = productsResultController.GetByName(settings.ManualUpdate);
                downloadProducts.AddRange(manualUpdate);
            }

            // reset updates between data source modifications
            productsResultController.ResetUpdated();

            // since we're done with all model modifications at this point - 
            // serialize and save on disk

            var gamesResultJson = "var data = " + jsonController.Stringify(productsResultController.ProductsResult);
            storage.Put(filename, gamesResultJson).Wait();

            // update product files for updated, new account products and manual updates

            var productFilesController = new ProductFilesController(
                downloadProducts,
                networkController,
                ioController,
                consoleController);

            productFilesController.UpdateFiles().Wait();

            // update images for all products

            var images = new ImagesController(networkController, ioController, consoleController);
            images.Update(productsResultController.ProductsResult).Wait();

            // nothing left to do here

            consoleController.WriteLine("All done. Press ENTER to quit...");
            consoleController.ReadLine();
        }
    }
}
