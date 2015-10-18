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
        static ISaveLoadDataHelper saveLoadHelper;

        #region Model variables

        static IList<Product> products = null;
        static IList<Product> owned = null;
        static IList<long> wishlisted = null;
        static IList<long> updated = null;
        static IList<ProductData> productsData = null;
        static IList<GameDetails> gamesDetails = null;

        static string[] gameDetailsLanguages = new string[1] { "English" };

        #endregion

        static void OnProductDataUpdated(object sender, ProductData data)
        {
            saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();
        }

        static void OnGameDetailsUpdated(object sender, GameDetails data)
        {
            saveLoadHelper.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();
        }

        static void OnBeforeGameDetailsAdding(ref GameDetails data, string item)
        {
            data.Id = long.Parse(item);
        }

        static void Main(string[] args)
        {
            #region IO variables

            var productTypesHelper = new ProductTypesHelper();

            var filenameTemplate = "{0}.js";
            var filenames = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(filenameTemplate);

            var prefixTemplate = "var {0}=";
            var prefixes = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(prefixTemplate);

            #endregion

            #region Shared IO controllers

            IConsoleController consoleController = new ConsoleController();
            IPostUpdateDelegate postUpdateDelegate = new ConsolePostUpdate(consoleController);

            IIOController ioController = new IOController();
            IStorageController<string> storageController = new StorageController(ioController);
            ISerializationController<string> jsonStringController = new JSONStringController();
            IUriController uriController = new UriController();
            IStringNetworkController networkController = new NetworkController(uriController);
            IFileRequestController fileRequestController = networkController as IFileRequestController;

            saveLoadHelper = new SaveLoadDataHelper(
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

            owned = saveLoadHelper.LoadData<List<Product>>(ProductTypes.Owned).Result;
            owned = owned ?? new List<Product>();

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

            IFilterDelegate<Product> existingProductsFilter = new ExistingProductsFilter();

            IRequestDelegate<Product> pagedResultController =
                new MultipageRequestController(
                    networkController,
                    jsonStringController,
                    postUpdateDelegate,
                    existingProductsFilter);

            IProductCoreController<Product> productsController = new ProductsController(products);

            IStringGetController gogDataController = new GOGDataController(networkController);

            ICollectionController<Product> ownedController = new OwnedController(owned);
            ICollectionController<long> updatedController = new UpdatedController(updated);

            IProductCoreController<ProductData> productsDataController = new ProductDataController(
                productsData,
                gogDataController,
                jsonStringController);

            IProductCoreController<GameDetails> gamesDetailsController = new GameDetailsController(
                gamesDetails,
                networkController,
                jsonStringController,
                gameDetailsLanguages);

            IProgress<double> downloadProgressReporter = new DownloadProgressReporter(consoleController);

            var productFilesController = new ProductFilesController(
                fileRequestController,
                ioController,
                consoleController,
                downloadProgressReporter);

            var imagesController = new ImagesController(
                fileRequestController,
                ioController,
                postUpdateDelegate);

            #endregion

            #region Load settings

            var settings = settingsController.Load().Result;

            #endregion

            #region Authorize on site

            if (!authorizationController.Authorize(settings).Result)
            {
                consoleController.WriteLine("Press ENTER to quit...");
                consoleController.ReadLine();
                return;
            }

            #endregion

            #region Get new products from gog.com/games

            //var productsFilter = new List<long>(products.Count);
            //foreach (var p in products) productsFilter.Add(p.Id);

            consoleController.Write("Requesting new products from {0}...", Urls.GamesAjaxFiltered);

            var newProducts = pagedResultController.Request(
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                products).Result;

            if (newProducts != null)
            {
                consoleController.Write("Got {0} new products.", newProducts.Count);

                for (var pp = newProducts.Count - 1; pp >= 0; pp--)
                    productsController.Insert(0, newProducts[pp]);
            }

            saveLoadHelper.SaveData(products, ProductTypes.Products).Wait();

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Get new owned products from gog.com/account

            consoleController.Write("Requesting new products from {0}...", Urls.AccountGetFilteredProducts);

            var newOwned = pagedResultController.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                owned).Result;

            if (newOwned != null)
            {
                consoleController.Write("Got {0} new products.", newOwned.Count);

                for (var oo = newOwned.Count - 1; oo >= 0; oo--)
                {
                    ownedController.Insert(0, newOwned[oo]);

                    // also add to updated list as we haven't downloaded them previously,
                    // so they need to be updated just like other files
                    updatedController.Add(newOwned[oo].Id);
                }
            }

            saveLoadHelper.SaveData(owned, ProductTypes.Owned).Wait();

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Get updated products

            consoleController.Write("Requesting updated products...");

            QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            var productUpdates = pagedResultController.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts).Result;

            if (productUpdates != null)
            {
                consoleController.Write("Got {0} updated products.", productUpdates.Count);

                foreach (var update in productUpdates)
                    updatedController.Add(update.Id);
            }

            saveLoadHelper.SaveData(updated, ProductTypes.Updated).Wait();

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Get wishlisted 

            consoleController.Write("Requesting wishlisted products...");

            var wishlistedString = gogDataController.GetString(Urls.Wishlist).Result;
            var wishlistedProductResult = jsonStringController.Deserialize<ProductsResult>(wishlistedString);

            if (wishlistedProductResult != null &&
                wishlistedProductResult.Products != null)
            {
                var count = wishlistedProductResult.Products.Count;

                consoleController.Write("Got {0} wishlisted products.", count);

                wishlisted = new List<long>(count);

                foreach (var wish in wishlistedProductResult.Products)
                    wishlisted.Add(wish.Id);
            }

            saveLoadHelper.SaveData(wishlisted, ProductTypes.Wishlisted).Wait();

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Update product data

            consoleController.Write("Updating product data...");

            productsDataController.OnProductUpdated += OnProductDataUpdated;

            var productWithoutProductData = new List<string>();

            foreach (var p in products)
            {
                var existingProductData = productsDataController.Find(p.Id);
                if (existingProductData != null) continue;

                if (string.IsNullOrEmpty(p.Url)) continue;

                productWithoutProductData.Add(p.Url);
            }

            productsDataController.Update(productWithoutProductData, postUpdateDelegate).Wait();

            saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();

            consoleController.WriteLine("DONE.");

            #endregion

            #region Update game details 

            consoleController.Write("Updating game details...");

            gamesDetailsController.OnProductUpdated += OnGameDetailsUpdated;
            gamesDetailsController.OnBeforeAdding += OnBeforeGameDetailsAdding;

            var ownedProductsWithoutGameDetails = new List<string>();

            foreach (var op in owned)
            {
                if (!updatedController.Contains(op.Id))
                {
                    var existingGameDetails = gamesDetailsController.Find(op.Id);
                    if (existingGameDetails != null) continue;
                }

                ownedProductsWithoutGameDetails.Add(op.Id.ToString());
            }

            gamesDetailsController.Update(ownedProductsWithoutGameDetails, postUpdateDelegate).Wait();

            saveLoadHelper.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();

            consoleController.WriteLine("DONE.");

            #endregion

            #region Download updated product files

            //var updatedProducts = new List<long>(updated);
            //foreach (var u in updatedProducts)
            //{
            //    var updatedGameDetails = gamesDetailsController.Find(u);
            //    productFilesController.UpdateFiles(updatedGameDetails, gameDetailsLanguages).Wait();

            //    updated.Remove(u);
            //    saveLoadHelper.SaveData(updated, ProductTypes.Updated).Wait();
            //}

            #endregion

            #region Update images

            consoleController.Write("Updating product images...");

            // for all products first
            imagesController.Update(products).Wait();

            // then for owned products that are not in a products collection 
            // (e.g. no longer sold, part of bundle)
            imagesController.Update(existingProductsFilter.Filter(owned, products)).Wait();

            consoleController.WriteLine("DONE.");

            #endregion

            #region Deauthorize on exit

            authorizationController.Deauthorize().Wait();

            #endregion

            consoleController.WriteLine("All done. Press ENTER to quit...");
            consoleController.ReadLine();
        }
    }
}
