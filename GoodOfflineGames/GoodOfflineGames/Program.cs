using System.Collections.Generic;

using GOG.SharedControllers;
using GOG.Controllers;
using GOG.Model;
using GOG.SharedModels;
using System.Threading.Tasks;

using GOG.Interfaces;
using System;

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

            #endregion

            #region GOG controllers

            var settingsController = new SettingsController(
                ioController,
                jsonStringController,
                consoleController);

            var authorizationController = new AuthorizationController(
                uriController,
                networkController,
                consoleController);

            var existingProductsFilter = new ExistingProductsFilter();

            var pagedResultController = new MultipageRequestController(
                    networkController,
                    jsonStringController,
                    consoleController,
                    existingProductsFilter);

            var productsController = new ProductsController(products);

            var gogDataController = new GOGDataController(networkController);

            var ownedController = new OwnedController(owned);
            var updatedController = new UpdatedController(updated);
            var productsDataController = new ProductsDataController(productsData);
            var gameDetailsController = new GameDetailsController(gamesDetails);

            #endregion

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

            //var productDataRequestTemplate = Urls.GameProductDataPageTemplate;

            //foreach (var p in products)
            //{
            //    var existingProductData = productsDataController.Find(p.Id);
            //    if (existingProductData != null) continue;

            //    if (string.IsNullOrEmpty(p.Url)) continue;

            //    var requestUri = string.Format(productDataRequestTemplate, p.Url);
            //    var productDataString = gogDataController.GetString(requestUri).Result;

            //    if (!string.IsNullOrEmpty(productDataString))
            //    {
            //        var gogData = jsonController.Parse<GOGData>(productDataString);
            //        if (gogData != null) productsDataController.Add(gogData.ProductData);
            //    }
            //}

            //saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();

            //#endregion

            #region Update game details 

            var gameDetailsRequestTemplate = Urls.AccountGameDetailsTemplate;

            //foreach (var p in products)
            //{
            //    if (!owned.Contains(p.Id)) continue;

            //    if (!updated.Contains(p.Id))
            //    {
            //        var existingGameDetails = gameDetailsController.Find(p.Id);
            //        if (existingGameDetails != null) continue;

            //        var existingProductData = productsDataController.Find(p.Id);
            //        if (existingProductData != null &&
            //            existingProductData.RequiredProducts != null &&
            //            existingProductData.RequiredProducts.Count > 0)
            //            continue;
            //    }

            //var requestUri = string.Format(gameDetailsRequestTemplate, 1430740694);
            //var gameDetailsString = networkController.GetString(requestUri).Result;

            var gameDetailsString = "{\"title\":\"Saints Row: The Third - The Full Package\",\"backgroundImage\":\"\\/\\/images-4.gog.com\\/e5c27c980df2ba2df51a08316cdecafc25bb932921eb9d1b8609f69476b91187\",\"cdKey\":\"\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_en\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/en1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/en1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"\\u010desk\\u00fd\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_cz\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/cz1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/cz1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"Deutsch\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_de\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/de1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/de1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"espa\\u00f1ol\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_es\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/es1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/es1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"fran\\u00e7ais\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_fr\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/fr1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/fr1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"italiano\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_it\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/it1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/it1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"nederlands\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_nl\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/nl1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/nl1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"polski\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_pl\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/pl1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/pl1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}],[\"\\u0440\\u0443\\u0441\\u0441\\u043a\\u0438\\u0439\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 1 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"33 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer6\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 2 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer7\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 3 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"3.9 GB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1installer8\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/installer_win_ru\",\"name\":\"Saints Row: The Third - The Full Package (Part 4 of 4)\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"481 MB\"},{\"manualUrl\":\"\\/downlink\\/saints_row_the_third_the_full_package\\/ru1patch1\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/ru1patch1\",\"name\":\"DLC blocker patch\",\"version\":\"(gog-5)\",\"date\":\"\",\"size\":\"1 MB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/saints_row_the_third_the_full_package\\/55363\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/55363\",\"name\":\"manual\",\"type\":\"manuals\",\"info\":1,\"size\":\"3 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/saints_row_the_third_the_full_package\\/55373\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/55373\",\"name\":\"wallpapers\",\"type\":\"wallpapers\",\"info\":7,\"size\":\"11 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/saints_row_the_third_the_full_package\\/55353\",\"downloaderUrl\":\"gogdownloader:\\/\\/saints_row_the_third_the_full_package\\/55353\",\"name\":\"avatars\",\"type\":\"avatars\",\"info\":12,\"size\":\"2 MB\"}],\"combinedExtrasDownloaderUrl\":[],\"dlcs\":[],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1431352200,\"messages\":[],\"changelog\":\"\\u003Ch4\\u003EDLC Patch (13.05.2015)\\u003C\\/h4\\u003E\\n\\u003Cul\\u003E\\n\\u003Cli\\u003EWe\\u0027ve added a patch that allows you to block certain overpowered DLC items from activating within the game. \\u003C\\/li\\u003E\\n\\u003Cli\\u003EPlease look for the \\u0026quot;DLC blocker patch\\u0026quot; in your account.\\u003C\\/li\\u003E\\n\\u003C\\/ul\\u003E\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/saints_row_series\"}";

            //    if (!string.IsNullOrEmpty(gameDetailsString))
            //    {


            var json = Newtonsoft.Json.JsonConvert.DeserializeObject(gameDetailsString);
            Console.WriteLine(json);
            //var gameDetails = 

            //var gameDetails = jsonStringController.Deserialize<GameDetails>(gameDetailsString);

            //if (gameDetails != null)
            //{
            //    // fix up. GOG started serving different JSON that doesn't work great with CLR JsonSerializer

            //    foreach (var entry in gameDetails.Downloads)
            //    {
            //        if (entry != null)
            //        {
            //            var array = entry as object[];
            //            if (array != null &&
            //                array.Length > 1)
            //            {
            //                if (array[0].ToString() == "English")
            //                {
            //                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //                    using (var ms = new System.IO.MemoryStream())
            //                    {
            //                        bf.Serialize(ms, array[1]);
            //                        var bytes = ms.ToArray();
            //                    }

            //                }
            //            }
            //        }
            //    }


            //    gameDetailsController.Add(gameDetails);
            //}


            //saveLoadHelper.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();

            //// update game details for all owned games
            //var gameDetailsProvider = new GameDetailsProvider(networkController, jsonController);
            //productsResultController.UpdateProductDetails(gameDetailsProvider, updatedProducts).Wait();

            #endregion

            #region Download updated product files

            var downloadDetails = new List<GameDetails>();

            //foreach (var u in updated)
            //    downloadDetails.Add(productsController.Find(u));

            var productFilesController = new ProductFilesController(
                fileRequestController,
                ioController,
                consoleController);

            productFilesController.UpdateFiles(downloadDetails).Wait();

            #endregion

            #region Update images

            var imagesController = new ImagesController(fileRequestController, ioController, consoleController);
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
