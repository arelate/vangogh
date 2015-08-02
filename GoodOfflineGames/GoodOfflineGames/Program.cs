using System.Collections.Generic;

using GOG.SharedControllers;
using GOG.Controllers;
using GOG.Models;
using GOG.SharedModels;
using GOG.Providers;

using GOG.Interfaces;
using System;

namespace GOG
{
    class PagedResultFilter : IPagedResultFilterDelegate
    {
        private IEnumerable<Product> baseSet;
        private IProductsController productsController;

        public PagedResultFilter(IEnumerable<Product> baseSet)
        {
            this.baseSet = baseSet;
            productsController = new ProductsController(baseSet, null, null, null, null);
        }

        public IEnumerable<Product> Filter(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                if (productsController.Find(product.Id) != null)
                {
                    yield return product;
                }
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            // IO

            IConsoleController consoleController = new ConsoleController();
            IIOController ioController = new IOController();
            var storage = new StorageController(ioController);
            IStringifyController jsonController = new JSONController();
            IUriController uriController = new UriController();
            IStringNetworkController networkController = new NetworkController(uriController);

            // GOG specific

            var settingsController = new SettingsController(
                ioController,
                jsonController,
                consoleController);
            IAuthenticationController authenticationController = new AuthenticationController(
                uriController,
                networkController,
                consoleController);

            var pagedResultController = new PagedResultController(networkController, jsonController);

            IList<Product> products = null;

            var jsonDataPrefix = "var data = ";
            var filename = "data.js";

            // Try to load stored games data and use it to update games rather than download again
            try
            {
                var storedGamesJson = storage.Pull(filename)
                    .Result
                    .Replace(jsonDataPrefix, string.Empty);
                var productResult = jsonController.Parse<ProductsResult>(storedGamesJson);
                products = productResult.Products;
            }
            catch
            {
                // file not found or couldn't be read
                products = new List<Product>();
            }

            var settings = settingsController.LoadSettings().Result;

            if (!authenticationController.Authorize(settings).Result)
            {
                consoleController.WriteLine("Press ENTER to exit...");
                consoleController.ReadLine();
                return;
            }

            var productsController = new ProductsController(
                products,
                networkController,
                jsonController,
                consoleController,
                pagedResultController);
            var dlcController = new DLCController();

            var ownedController = new OwnedController(productsController, dlcController);
            var wishlistController = new WishlistController(productsController, networkController, jsonController);

            // get all owned games from gog.com/account

            // filter owned products, so that we can check if account has got new owned games,
            // and not just new games, also this is critical as we mark games as owned later

            var storedOwned = ownedController.GetOwned();
            var storedFilter = new PagedResultFilter(products);
            var ownedFilter = new PagedResultFilter(storedOwned);

            // get all games owned by user
            var newOwned = productsController.Get(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                ownedFilter,
                "Getting new account games...").Result;

            // get all available games from gog.com/games
            var newAvailable = productsController.Get(
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                storedFilter,
                "Getting new games available on GOG.com...").Result;

            Console.WriteLine(newOwned.ToString() + newAvailable.ToString());

            //// separately get all updated products
            //QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            //var updatedProducts = productsResultController.GetAll(
            //    Urls.AccountGetFilteredProducts,
            //    QueryParameters.AccountGetFilteredProducts,
            //    "Getting updated games...").Result;

            //// mark all new owned games as owned
            //productsResultController.SetAllAsOwned(newOwned);

            //// merge owned games into all available games 
            //productsResultController.MergeOwned(newOwned);

            ////// merge updated games into owned games
            ////productsResultController.MergeUpdated(updatedResult);

            //// update details for individual games
            //var gogDataController = new GOGDataController(networkController);

            //// update product data from game pages
            //var productDataProvider = new ProductDataProvider(gogDataController, jsonController);
            //productsResultController.UpdateProductDetails(productDataProvider).Wait();

            //// update game details for all owned games
            //var gameDetailsProvider = new GameDetailsProvider(networkController, jsonController);
            //productsResultController.UpdateProductDetails(gameDetailsProvider, updatedProducts).Wait();

            //var wishlistController = new WishlistController(gogDataController, jsonController);
            //// clear wishlisted before setting new ones
            //productsResultController.ClearWishlisted();

            //// update wishlisted games
            //var wishlistResult = wishlistController.RequestWishlisted(consoleController).Result;

            //productsResultController.MergeWishlisted(wishlistResult);

            //// we'll drive downloads with product titles
            //// as for updated products and new account products 
            //// when we got those lists the result likely don't have product details
            //// that contain download links; using product titles 
            //// we'll query stored products that have details and get links
            //var downloadProductsNames = new List<string>();

            //// get product files for updated and new files
            ////var updatedProducts = productsResultController.GetUpdated();
            //foreach (var p in updatedProducts)
            //{
            //    downloadProductsNames.Add(p.Title);
            //}

            //// when we got updated owned products they didn't have download links,
            //// since then we've updated all product details and productsResultController should have
            //// all the details we need - so for every newly obtained product we need to find match 
            //// in current productsReusltController to use for updating files
            //foreach (var uo in newOwned)
            //{
            //    downloadProductsNames.Add(uo.Title);
            //}

            //// also if we've passed products for manual update through settings.json
            //// add them for update as well
            //if (settings.ManualUpdate != null)
            //{
            //    downloadProductsNames.AddRange(settings.ManualUpdate);
            //}

            //// now actually create list of product that need product files update
            //var downloadProducts = productsResultController.GetByName(downloadProductsNames);

            ////// reset updates between data source modifications
            ////productsResultController.ResetUpdated();

            //// since we're done with all model modifications at this point - 
            //// serialize and save on disk

            //var gamesResultJson = "var data = " + jsonController.Stringify(productsResultController.ProductsResult);
            //storage.Put(filename, gamesResultJson).Wait();

            //// update product files for updated, new account products and manual updates

            //var productFilesController = new ProductFilesController(
            //    downloadProducts,
            //    networkController,
            //    ioController,
            //    consoleController);

            //productFilesController.UpdateFiles().Wait();

            //// update images for all products

            //var images = new ImagesController(networkController, ioController, consoleController);
            //images.Update(productsResultController.ProductsResult).Wait();

            // nothing left to do here

            consoleController.WriteLine("All done. Press ENTER to quit...");
            consoleController.ReadLine();
        }
    }
}
