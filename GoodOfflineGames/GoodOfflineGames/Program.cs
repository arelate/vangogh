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

            string[] gameDetailsLanguages = new string[1] { "English" };

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
