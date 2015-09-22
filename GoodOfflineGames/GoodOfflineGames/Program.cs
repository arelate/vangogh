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

            //var gameDetailsRequestTemplate = Urls.AccountGameDetailsTemplate;

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

            //    var requestUri = string.Format(gameDetailsRequestTemplate, p.Id);
            //    //var gameDetailsString = networkController.GetString(requestUri).Result;
            //    var gameDetailsString = "{\"title\":\"The Incredible Adventures of Van Helsing II - Complete Pack\",\"backgroundImage\":\"\\/\\/images-3.gog.com\\/a2e80caaf4b6eb543e24243d6c5c407b6eb851f2b4dbf811c0d244e7f1abbc8d\",\"cdKey\":\"\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/en1installer1\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/installer_win_en\",\"name\":\"The Incredible Adventures of Van Helsing II - Complete Pack (Part 1 of 5)\",\"version\":\"1.3.4b (gog-1)\",\"date\":\"\",\"size\":\"36 MB\"},{\"manualUrl\":\"\\/downlink\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/en1installer2\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/installer_win_en\",\"name\":\"The Incredible Adventures of Van Helsing II - Complete Pack (Part 2 of 5)\",\"version\":\"1.3.4b (gog-1)\",\"date\":\"\",\"size\":\"4.2 GB\"},{\"manualUrl\":\"\\/downlink\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/en1installer3\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/installer_win_en\",\"name\":\"The Incredible Adventures of Van Helsing II - Complete Pack (Part 3 of 5)\",\"version\":\"1.3.4b (gog-1)\",\"date\":\"\",\"size\":\"4.2 GB\"},{\"manualUrl\":\"\\/downlink\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/en1installer4\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/installer_win_en\",\"name\":\"The Incredible Adventures of Van Helsing II - Complete Pack (Part 4 of 5)\",\"version\":\"1.3.4b (gog-1)\",\"date\":\"\",\"size\":\"4.2 GB\"},{\"manualUrl\":\"\\/downlink\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/en1installer5\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/installer_win_en\",\"name\":\"The Incredible Adventures of Van Helsing II - Complete Pack (Part 5 of 5)\",\"version\":\"1.3.4b (gog-1)\",\"date\":\"\",\"size\":\"468 MB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/59763\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/59763\",\"name\":\"wallpaper\",\"type\":\"wallpapers\",\"info\":1,\"size\":\"2 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/59753\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/59753\",\"name\":\"soundtrack\",\"type\":\"audio\",\"info\":1,\"size\":\"89 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/59773\",\"downloaderUrl\":\"gogdownloader:\\/\\/the_incredible_adventures_of_van_helsing_ii_complete_pack\\/59773\",\"name\":\"avatar\",\"type\":\"avatars\",\"info\":1,\"size\":\"1 MB\"}],\"combinedExtrasDownloaderUrl\":[],\"dlcs\":[],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1441975800,\"messages\":[],\"changelog\":\"\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/van_helsing_series\"}";

            //    if (!string.IsNullOrEmpty(gameDetailsString))
            //    {

            //        Newtonsoft.Json.JsonTextReader jtr = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(gameDetailsString));
            //        while(jtr.Read())
            //        {
            //            if (jtr.Value != null) {
            //                Console.WriteLine("{0}:{1}:{2}", jtr.TokenType, jtr.ValueType, jtr.Value);
            //            }
            //        }


            //        var gameDetails = jsonStringController.Deserialize<GameDetails>(gameDetailsString);

            //        if (gameDetails != null)
            //        {
            //            // fix up. GOG started serving different JSON that doesn't work great with CLR JsonSerializer

            //            foreach (var entry in gameDetails.DownloadObjects)
            //            {
            //                //if (entry != null)
            //                //{
            //                //    var array = entry as object[];
            //                //    if (array != null &&
            //                //        array.Length > 1)
            //                //    {
            //                //        if (array[0].ToString() == "English")
            //                //        {
            //                            Console.WriteLine(entry);
            //                        //}
            //                    //}
            //                //}
            //            }


            //            gameDetailsController.Add(gameDetails);
            //        }
            //    }
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
