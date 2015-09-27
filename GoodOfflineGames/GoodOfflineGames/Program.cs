using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GOG.SharedControllers;
using GOG.Controllers;
using GOG.Model;
using GOG.SharedModels;

using GOG.Interfaces;

namespace GOG
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Model variables

            IList<Product> products = null;
            IList<long> owned = null;
            IList<long> wishlisted = null;
            IList<long> updated = null;
            IList<ProductData> productsData = null;
            IList<GameDetails> gamesDetails = null;

            List<string> gameDetailsLanguages = new List<string>() { "English" };

            #endregion

            #region IO variables

            var productTypesHelper = new ProductTypesHelper();

            var filenameTemplate = "{0}.js";
            var filenames = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(filenameTemplate);

            var prefixTemplate = "var {0}=";
            var prefixes = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(prefixTemplate);

            #endregion

            #region Shared IO controllers

            IConsoleController consoleController = new ConsoleController();
            IIOController ioController = new IOController();
            IStorageController<string> storageController = new StorageController(ioController);
            ISerializationController<string> jsonStringController = new JSONStringController();
            IUriController uriController = new UriController();
            IStringNetworkController networkController = new NetworkController(uriController);
            IFileRequestController fileRequestController = networkController as IFileRequestController;

            var saveLoadHelper = new SaveLoadDataHelper(
                storageController,
                jsonStringController,
                filenames,
                prefixes);

            #endregion

            #region Loading stored data

            consoleController.Write("Loading stored data...");

            // Load stored data for products, owned, wishlist and queued updates
            products = saveLoadHelper.LoadData<List<Product>>(ProductTypes.Products).Result;
            products = products ?? new List<Product>();

            owned = saveLoadHelper.LoadData<List<long>>(ProductTypes.Owned).Result;
            owned = owned ?? new List<long>();

            updated = saveLoadHelper.LoadData<List<long>>(ProductTypes.Updated).Result;
            updated = updated ?? new List<long>();

            productsData = saveLoadHelper.LoadData<List<ProductData>>(ProductTypes.ProductsData).Result;
            productsData = productsData ?? new List<ProductData>();

            gamesDetails = saveLoadHelper.LoadData<List<GameDetails>>(ProductTypes.GameDetails).Result;
            gamesDetails = gamesDetails ?? new List<GameDetails>();

            consoleController.WriteLine("DONE.");

            #endregion

            #region GOG controllers

            ISettingsController<Settings> settingsController = new SettingsController(
                ioController,
                jsonStringController,
                consoleController);

            IAuthorizationController authorizationController = new AuthorizationController(
                uriController,
                networkController,
                consoleController);

            IFilterDelegate<Product, long> existingProductsFilter = new ExistingProductsFilter();

            IRequestDelegate<Product, long> pagedResultController = 
                new MultipageRequestController(
                    networkController,
                    jsonStringController,
                    consoleController,
                    existingProductsFilter);

            IProductCoreController<Product> productsController = new ProductsController(products);

            IStringGetController gogDataController = new GOGDataController(networkController);

            ICollectionController<long> ownedController = new OwnedController(owned);
            ICollectionController<long> updatedController = new UpdatedController(updated);

            IProductCoreController<ProductData> productsDataController = new ProductDataController(
                productsData,
                productsController,
                gogDataController,
                jsonStringController);

            IProductCoreController<GameDetails> gamesDetailsController = new GameDetailsController(
                gamesDetails,
                productsController,
                networkController,
                jsonStringController,
                ownedController,
                updatedController,
                productsDataController,
                gameDetailsLanguages);

            var productFilesController = new ProductFilesController(
                fileRequestController,
                ioController,
                consoleController);

            var imagesController = new ImagesController(
                fileRequestController,
                ioController,
                consoleController);

            #endregion

            //var data = gogDataController.GetString(string.Format(Urls.GameProductDataPageTemplate, "/game/system_shock_enhanced_edition")).Result;
            //Console.WriteLine(data);

            //#region Load settings

            //var settings = settingsController.Load().Result;

            //#endregion

            //#region Authorize on site

            //if (!authorizationController.Authorize(settings).Result)
            //{
            //    consoleController.WriteLine("Press ENTER to exit...");
            //    consoleController.ReadLine();
            //    return;
            //}

            //#endregion

            //#region Get new products from gog.com/games

            //var productsFilter = new List<long>(products.Count);
            //foreach (var p in products) productsFilter.Add(p.Id);

            //consoleController.Write("Requesting new products from {0}...", Urls.GamesAjaxFiltered);

            //var newProducts = pagedResultController.Request(
            //    Urls.GamesAjaxFiltered,
            //    QueryParameters.GamesAjaxFiltered,
            //    productsFilter).Result;

            //if (newProducts != null)
            //{
            //    consoleController.Write("Got {0} new products.", newProducts.Count);

            //    for (var pp = newProducts.Count - 1; pp >= 0; pp--)
            //        productsController.Insert(0, newProducts[pp]);
            //}

            //saveLoadHelper.SaveData(products, ProductTypes.Products).Wait();

            //consoleController.WriteLine(string.Empty);

            //#endregion

            //#region Get new owned products from gog.com/account

            //consoleController.Write("Requesting new products from {0}...", Urls.AccountGetFilteredProducts);

            //var newOwned = pagedResultController.Request(
            //    Urls.AccountGetFilteredProducts,
            //    QueryParameters.AccountGetFilteredProducts,
            //    owned).Result;

            //if (newOwned != null)
            //{
            //    consoleController.Write("Got {0} new products.", newOwned.Count);

            //    for (var oo = newOwned.Count - 1; oo >= 0; oo--)
            //        ownedController.Insert(0, newOwned[oo].Id);
            //}

            //saveLoadHelper.SaveData(owned, ProductTypes.Owned).Wait();

            //consoleController.WriteLine(string.Empty);

            //#endregion

            //#region Get updated products

            //consoleController.Write("Requesting updated products...");

            //QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            //var productUpdates = pagedResultController.Request(
            //    Urls.AccountGetFilteredProducts,
            //    QueryParameters.AccountGetFilteredProducts).Result;

            //if (productUpdates != null)
            //{
            //    consoleController.Write("Got {0} updated products.", productUpdates.Count);

            //    foreach (var update in productUpdates)
            //        updatedController.Add(update.Id);
            //}

            //saveLoadHelper.SaveData(updated, ProductTypes.Updated).Wait();

            //consoleController.WriteLine(string.Empty);

            //#endregion

            //#region Get wishlisted 

            //consoleController.Write("Requesting wishlisted products...");

            //var wishlistedString = gogDataController.GetString(Urls.Wishlist).Result;
            //var wishlistedProductResult = jsonController.Parse<ProductsResult>(wishlistedString);

            //if (wishlistedProductResult != null &&
            //    wishlistedProductResult.Products != null)
            //{
            //    var count = wishlistedProductResult.Products.Count;

            //    consoleController.Write("Got {0} wishlisted products.", count);

            //    wishlisted = new List<long>(count);

            //    foreach (var wish in wishlistedProductResult.Products)
            //        wishlisted.Add(wish.Id);
            //}

            //saveLoadHelper.SaveData(wishlisted, ProductTypes.Wishlisted).Wait();

            //consoleController.WriteLine(string.Empty);

            //#endregion

            //#region Update product data

            consoleController.Write("Updating product data...");

            //productsDataController.Update(consoleController);

            //saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();

            consoleController.WriteLine("DONE.");

            //#endregion

            #region Update game details 

            // saints row the third 
            //var gameDetailsString = "{\"title\":\"Saints Row: The Third - The Full Package\",\"backgroundImage\":\"\\/\\/images-4.gog.com\\/e5c27c980df2ba2df51a08316cdecafc25bb932921eb9d1b8609f69476b91187\",\"cdKey\":\"\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/en1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"\\u010desk\\u00fd\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/cz1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"Deutsch\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/de1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"espa\\u00f1ol\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/es1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"fran\\u00e7ais\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/fr1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"italiano\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/it1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"nederlands\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/nl1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"polski\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/pl1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"\\u0440\\u0443\\u0441\\u0441\\u043a\\u0438\\u0439\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/ru1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/saints_row_the_third_the_full_package\\/55363\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/55363\",\"name\":\"manual\",\"type\":\"manuals\",\"info\":1,\"size\":\"3 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/saints_row_the_third_the_full_package\\/55373\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/55373\",\"name\":\"wallpapers\",\"type\":\"wallpapers\",\"info\":7,\"size\":\"11 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/saints_row_the_third_the_full_package\\/55353\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/55353\",\"name\":\"avatars\",\"type\":\"avatars\",\"info\":12,\"size\":\"2 MB\"}],\"combinedExtrasDownloaderUrl\":[],\"dlcs\":[],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1431352200,\"messages\":[],\"changelog\":\"\\u003Ch4\\u003EDLC Patch (13.05.2015)\\u003C\\/h4\\u003E\\n\\u003Cul\\u003E\\n\\u003Cli\\u003EWe\\u0027ve added a patch that allows you to block certain overpowered DLC items from activating within the game. \\u003C\\/li\\u003E\\n\\u003Cli\\u003EPlease look for the \\u0026quot;DLC blocker patch\\u0026quot; in your account.\\u003C\\/li\\u003E\\n\\u003C\\/ul\\u003E\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/saints_row_series\"}";

            // darksiders 2
            //var gameDetailsString = "{\"title\":\"Darksiders II\",\"backgroundImage\":\"\\/\\/images-3.gog.com\\/ebbb60262e9f92813917cba3b92820cf07465abcafb87143f65c7cc2c193195e\",\"cdKey\":\"\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/darksiders_ii\\/en1installer1\",\"downloaderUrl\":\"gogdownloader:\\/\\/darksiders_ii\\/installer_win_en\",\"name\":\"Darksiders II (Part 1 of 3)\",\"version\":null,\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/darksiders_ii\\/en1installer2\",\"downloaderUrl\":\"gogdownloader:\\/\\/darksiders_ii\\/installer_win_en\",\"name\":\"Darksiders II (Part 2 of 3)\",\"version\":null,\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/darksiders_ii\\/en1installer3\",\"downloaderUrl\":\"gogdownloader:\\/\\/darksiders_ii\\/installer_win_en\",\"name\":\"Darksiders II (Part 3 of 3)\",\"version\":null,\"date\":\"\",\"size\":\"3.5 GB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/darksiders_ii\\/55343\",\"downloaderUrl\":\"gogdownloader:\\/\\/darksiders_ii\\/55343\",\"name\":\"manual\",\"type\":\"manuals\",\"info\":1,\"size\":\"1 MB\"}],\"combinedExtrasDownloaderUrl\":[],\"dlcs\":[{\"title\":\"Darksiders II - Complete DLC Pack\",\"backgroundImage\":\"\\/\\/images-1.gog.com\\/4eabc4be295126a5bc9a33c724a54ab784497209582a1020bc45950779dc6971\",\"cdKey\":\"\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/darksiders_ii_complete_dlc_pack\\/en1installer1\",\"downloaderUrl\":\"gogdownloader:\\/\\/darksiders_ii_complete_dlc_pack\\/installer_win_en\",\"name\":\"DLC\",\"version\":null,\"date\":\"\",\"size\":\"733 MB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/darksiders_ii_complete_dlc_pack\\/55383\",\"downloaderUrl\":\"gogdownloader:\\/\\/darksiders_ii_complete_dlc_pack\\/55383\",\"name\":\"soundtrack (MP3)\",\"type\":\"audio\",\"info\":1,\"size\":\"150 MB\"}],\"combinedExtrasDownloaderUrl\":[],\"dlcs\":[],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1431324000,\"messages\":[],\"changelog\":\"\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/darksiders_series\"}],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1431324000,\"messages\":[],\"changelog\":\"\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/darksiders_series\"}";

            consoleController.Write("Updating game details...");

            gamesDetailsController.Update(consoleController).Wait();

            saveLoadHelper.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();

            consoleController.WriteLine("DONE.");

            #endregion

            #region Download updated product files

            var downloadDetails = new List<GameDetails>();

            // TODO: add new products as well

            foreach (var u in updated)
                downloadDetails.Add(gamesDetailsController.Find(u));

            productFilesController.UpdateFiles(downloadDetails, gameDetailsLanguages).Wait();

            #endregion

            #region Update images

            imagesController.Update(products).Wait();

            #endregion

            #region Deauthorize on exit

            authorizationController.Deauthorize().Wait();

            #endregion

            consoleController.WriteLine("All done. Press ENTER to quit...");
            consoleController.ReadLine();
        }
    }
}
