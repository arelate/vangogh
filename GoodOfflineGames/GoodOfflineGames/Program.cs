using System;
using System.Collections.Generic;

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

        static IDictionary<long, DateTime> checkedOwned = null;
        static IDictionary<long, DateTime> checkedProductData = null;

        static IList<ProductFile> productFiles = null;

        #endregion

        static void OnProductDataUpdated(object sender, ProductData data)
        {
            saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();

            if (!checkedProductData.ContainsKey(data.Id)) checkedProductData.Add(data.Id, DateTime.Today);
            else checkedProductData[data.Id] = DateTime.Today;

            saveLoadHelper.SaveData(checkedProductData, ProductTypes.CheckedProductData).Wait();
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

            var recycleBin = "_RecycleBin";

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

            checkedOwned = saveLoadHelper.LoadData<Dictionary<long, DateTime>>(ProductTypes.CheckedOwned).Result;
            checkedOwned = checkedOwned ?? new Dictionary<long, DateTime>();

            checkedProductData = saveLoadHelper.LoadData<Dictionary<long, DateTime>>(ProductTypes.CheckedProductData).Result;
            checkedProductData = checkedProductData ?? new Dictionary<long, DateTime>();

            productFiles = saveLoadHelper.LoadData<List<ProductFile>>(ProductTypes.ProductFiles).Result;
            productFiles = productFiles ?? new List<ProductFile>();

            consoleController.WriteLine("DONE.");

            #endregion

            #region Load settings

            ISettingsController<Settings> settingsController = new SettingsController(
                ioController,
                jsonStringController,
                consoleController);

            var settings = settingsController.Load().Result;

            #endregion

            #region GOG controllers

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
                settings.DownloadLanguages);

            IProgress<double> downloadProgressReporter = new DownloadProgressReporter(consoleController);

            var productFilesDownloadController = new ProductFilesDownloadController(
                fileRequestController,
                ioController,
                consoleController,
                downloadProgressReporter);

            var imagesController = new ImagesController(
                fileRequestController,
                ioController,
                postUpdateDelegate);

            #endregion

            #region Authorize on site

            if (!authorizationController.Authorize(settings).Result)
            {
                consoleController.WriteLine("Press ENTER to quit...");
                consoleController.ReadLine();
                return;
            }

            #endregion

            #region Get new products from gog.com/games and gog.com/account

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

            #region Get wishlisted products

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

            var dataCheckThresholdDays = 30;

            var productsWithoutProductData = new List<string>();

            foreach (var p in products)
            {
                var existingProductData = productsDataController.Find(p.Id);
                if (existingProductData != null)
                {
                    // only skip data known to be checked within last 30 days
                    if (checkedProductData.ContainsKey(p.Id) &&
                        (DateTime.Today - checkedProductData[p.Id]).Days < dataCheckThresholdDays)
                    {
                        continue;
                    }
                }

                if (string.IsNullOrEmpty(p.Url)) continue;

                productsWithoutProductData.Add(p.Url);
            }

            var newProductData = productsDataController.Update(productsWithoutProductData, postUpdateDelegate).Result;
            saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();

            // after updating product data for new products, we need to check if any of the new products is DLC
            // and if it is - add required product to updated and update product data for that product

            var productsWithNewDLC = new List<string>();

            foreach (var pData in newProductData)
            {
                // this is a DLC, so let's add RequiredProduct (if owned) for update to download new DLC
                // also we need to update RequiredProducts parent data as well
                if (pData.RequiredProducts != null &&
                    pData.RequiredProducts.Count > 0)
                    foreach (var rp in pData.RequiredProducts)
                    {
                        var productWithNewDLC = productsController.Find(rp.Id);

                        // only add parent product if it's owned
                        if (ownedController.Contains(productWithNewDLC) &&
                            !updated.Contains(rp.Id))
                            updated.Add(rp.Id);

                        if (string.IsNullOrEmpty(productWithNewDLC.Url)) continue;

                        productsWithNewDLC.Add(productWithNewDLC.Url);
                    }
            }

            saveLoadHelper.SaveData(updated, ProductTypes.Updated).Wait();

            if (productsWithNewDLC.Count > 0)
            {
                productsDataController.Update(productsWithNewDLC, postUpdateDelegate).Wait();
                saveLoadHelper.SaveData(productsData, ProductTypes.ProductsData).Wait();
            }

            consoleController.WriteLine("DONE.");

            #endregion

            #region Enforce data consistency so that DLCs have proper ids

            consoleController.Write("Enforcing data consistency for DLCs...");

            var dataConsistencyController = new DataConsistencyController(gamesDetails, productsData);
            if (dataConsistencyController.Update())
            {
                // only save to disk if we've indeed updated gamesDetails
                // which shouldn't happen all updates but rather 
                // only when user obtains new product with owned DLC or just DLC
                saveLoadHelper.SaveData(gamesDetails, ProductTypes.GameDetails);
            }

            consoleController.WriteLine("DONE.");

            #endregion

            #region Update images

            consoleController.Write("Updating product images...");

            // for all products first
            imagesController.Update(products).Wait();

            // then for owned products that are not in a products collection 
            // (e.g. no longer sold, part of bundle)
            imagesController.Update(existingProductsFilter.Filter(owned, products)).Wait();

            imagesController.Update(productsData).Wait();

            consoleController.WriteLine("DONE.");

            #endregion

            #region Update products - game details, download and cleanup folders

            consoleController.WriteLine("Updating game details, product files and cleaning up product folders...");

            var updateAllThrottleMinutes = 2; // 2 minutes
            var updateAllThrottleMilliseconds = 1000 * 60 * updateAllThrottleMinutes;

            gamesDetailsController.OnProductUpdated += OnGameDetailsUpdated;
            gamesDetailsController.OnBeforeAdding += OnBeforeGameDetailsAdding;

            var ownedProductsWithoutGameDetails = new List<string>();

            // clone the collection since we'll be removing items from it
            var updatedProducts = (settings.UpdateAll) ?
                new List<long>(owned.Count) :
                new List<long>(updated);

            if (settings.UpdateAll)
                foreach (var o in owned)
                    updatedProducts.Add(o.Id);

            var failedAttempts = 0;
            var failedAttemptThreshold = 2;

            foreach (var u in updatedProducts)
            {
                // fast bail on not owned products
                var product = productsController.Find(u);
                if (product != null &&
                    !ownedController.Contains(product) &&
                    updated.Contains(u))
                {
                    consoleController.WriteLine("WARNING: Product {0} is not owned and couldn't be updated, removing it from updates.");
                    updated.Remove(u);
                    saveLoadHelper.SaveData(updated, ProductTypes.Updated).Wait();
                    continue;
                }

                // we use single loop to:
                // 1) update game details
                // 2) download product updates
                // 3) store product files
                // 4) cleanup product folder
                // 5) remove from updated list   
                // this is done to avoid spending all request limit on GOG.com
                // just updating game details and not being able to update files
                // which is highly likely in case of many updates

                if (settings.UpdateAll)
                {
                    // check if we've checked same game within last 30 days
                    if (checkedOwned.ContainsKey(u))
                    {
                        var checkedDays = DateTime.Today - checkedOwned[u];
                        if (checkedDays.Days < dataCheckThresholdDays)
                        {
                            consoleController.WriteLine("Product {0} already checked within last {1} days.",
                                u, dataCheckThresholdDays);
                            continue;
                        }
                    }
                }

                // request updated game details
                consoleController.Write("Updating game details for {0}...", u);

                gamesDetailsController.Update(new List<string> { u.ToString() }).Wait();

                // save new details
                saveLoadHelper.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();

                consoleController.WriteLine("DONE.");

                // request updated product files
                var updatedGameDetails = gamesDetailsController.Find(u);

                var productIntallersExtras =
                    productFilesDownloadController.UpdateFiles(
                        updatedGameDetails,
                        settings.DownloadLanguages,
                        settings.DownloadOperatingSystems).Result;

                IProductFileController productFilesController = new ProductFilesController(productIntallersExtras);

                // product files contain all files for that product, to store them:
                // - remove all entries for that id from productFiles
                productFiles = productFilesController.Filter(productFiles);
                // - add those new entries to productFiles
                foreach (var productFile in productIntallersExtras)
                    productFiles.Add(productFile);
                // - write updated product files
                saveLoadHelper.SaveData(productFiles, ProductTypes.ProductFiles);

                // remove from updated and cleanup only if all files were downloaded successfully
                if (productFilesController.CheckSuccess())
                {
                    // remove update entry as all files have been downloaded
                    if (updated.Contains(u))
                    {
                        updated.Remove(u);
                        saveLoadHelper.SaveData(updated, ProductTypes.Updated).Wait();
                    }

                    if (settings.CleanupProductFolders)
                    {
                        // cleanup product folder
                        consoleController.Write("Cleaning up product folder...");

                        ICleanupController cleanupController =
                            new CleanupController(
                                productFilesController,
                                ioController);

                        cleanupController.Cleanup(recycleBin);
                    }
                }
                else
                {
                    if (++failedAttempts >= failedAttemptThreshold)
                    {
                        consoleController.WriteLine("Last {0} attempts to download product files were not successful. " +
                            "Recommended: wait 12-24 hours and try again. Abandoning attempts.", failedAttempts);
                        break;
                    }
                }

                if (!checkedOwned.ContainsKey(u)) checkedOwned.Add(u, DateTime.Today);
                else checkedOwned[u] = DateTime.Today;

                saveLoadHelper.SaveData(checkedOwned, ProductTypes.CheckedOwned).Wait();

                consoleController.WriteLine("DONE.");

                // throttle server access
                if (settings.UpdateAll)
                {
                    Console.WriteLine("Waiting {0} minute(s) before next request...", updateAllThrottleMinutes);
                    System.Threading.Thread.Sleep(updateAllThrottleMilliseconds);
                }
            }

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
