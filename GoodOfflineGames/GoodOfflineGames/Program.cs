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
        static ISaveLoadDataController saveLoadDataController;

        #region Model variables

        static IList<Product> products = null;
        static IList<Product> owned = null;
        static IList<long> wishlisted = null;
        static IList<long> updated = null;
        static IList<ProductData> productsData = null;
        static IList<GameDetails> gamesDetails = null;
        static IDictionary<long, List<string>> screenshots = null;

        static IDictionary<long, DateTime> checkedOwned = null;
        static IDictionary<long, DateTime> checkedProductData = null;

        static IList<ProductFile> productFiles = null;

        #endregion

        static void OnProductDataUpdated(object sender, ProductData data)
        {
            saveLoadDataController.SaveData(productsData, ProductTypes.ProductsData).Wait();

            if (!checkedProductData.ContainsKey(data.Id)) checkedProductData.Add(data.Id, DateTime.Today);
            else checkedProductData[data.Id] = DateTime.Today;

            saveLoadDataController.SaveData(checkedProductData, ProductTypes.CheckedProductData).Wait();
        }

        static void OnGameDetailsUpdated(object sender, GameDetails data)
        {
            saveLoadDataController.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();
        }

        static void OnBeforeGameDetailsAdding(ref GameDetails data, string item)
        {
            data.Id = long.Parse(item);
        }

        static void Main(string[] args)
        {
            var dataCheckThresholdDays = 30;

            #region IO variables

            var productTypesHelper = new ProductTypesHelper();

            var filenameTemplate = "{0}.js";
            var filenames = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(filenameTemplate);

            var prefixTemplate = "var {0}=";
            var prefixes = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(prefixTemplate);

            var recycleBin = "_RecycleBin";
            var imagesFolder = "_images";
            var screenshotsFolder = "_screenshots";
            var logFilename = "log.txt";

            #endregion

            #region Shared IO controllers

            IConsoleController consoleController = new ConsoleController();
            // at this point we're not sure whether we'll be requested to write to log or not...
            IDisposableConsoleController loggingConsoleController = new ConsoleController();
            IPostUpdateDelegate postUpdateDelegate = new ConsolePostUpdate(consoleController);

            IIOController ioController = new IOController();

            IStorageController<string> storageController = new StorageController(ioController);
            ISerializationController<string> jsonStringController = new JSONStringController();
            IUriController uriController = new UriController();
            IStringNetworkController networkController = new NetworkController(uriController);
            IRequestFileDelegate requestFileDelegate = networkController as IRequestFileDelegate;
            ITokenExtractorController tokenExtractorController = new TokenExtractorController();

            ILanguageCodesController languageCodesController = new LanguageCodesController();

            saveLoadDataController = new SaveLoadDataHelper(
                storageController,
                jsonStringController,
                filenames,
                prefixes);

            #endregion

            #region Load settings

            ISettingsController<Settings> settingsController = new SettingsController(
                ioController,
                jsonStringController,
                languageCodesController,
                consoleController);

            var settings = settingsController.Load().Result;

            if (settings.UseLog)
            {
                // now we're sure, so creating a file log passthtough console controller
                loggingConsoleController = new LoggingConsoleController(logFilename, consoleController);
            }

            #endregion

            #region Loading stored data

            loggingConsoleController.WriteLine("Starting update session {0}.", ConsoleColor.White, DateTime.Now);

            loggingConsoleController.Write("Loading stored data...", ConsoleColor.Gray);

            // Load stored data for products, owned, wishlist and queued updates
            products = saveLoadDataController.LoadData<List<Product>>(ProductTypes.Products).Result;
            products = products ?? new List<Product>();

            owned = saveLoadDataController.LoadData<List<Product>>(ProductTypes.Owned).Result;
            owned = owned ?? new List<Product>();

            updated = saveLoadDataController.LoadData<List<long>>(ProductTypes.Updated).Result;
            updated = updated ?? new List<long>();

            productsData = saveLoadDataController.LoadData<List<ProductData>>(ProductTypes.ProductsData).Result;
            productsData = productsData ?? new List<ProductData>();

            gamesDetails = saveLoadDataController.LoadData<List<GameDetails>>(ProductTypes.GameDetails).Result;
            gamesDetails = gamesDetails ?? new List<GameDetails>();

            checkedOwned = saveLoadDataController.LoadData<Dictionary<long, DateTime>>(ProductTypes.CheckedOwned).Result;
            checkedOwned = checkedOwned ?? new Dictionary<long, DateTime>();

            checkedProductData = saveLoadDataController.LoadData<Dictionary<long, DateTime>>(ProductTypes.CheckedProductData).Result;
            checkedProductData = checkedProductData ?? new Dictionary<long, DateTime>();

            productFiles = saveLoadDataController.LoadData<List<ProductFile>>(ProductTypes.ProductFiles).Result;
            productFiles = productFiles ?? new List<ProductFile>();

            screenshots = saveLoadDataController.LoadData<Dictionary<long, List<string>>>(ProductTypes.Screenshots).Result;
            screenshots = screenshots ?? new Dictionary<long, List<string>>();

            loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

            #endregion

            #region Debug only - print all known folders and exit

            /*
            //var dirs = new List<string>();
            //foreach (var pf in productFiles)
            //{
            //    if (!dirs.Contains(pf.Folder)) dirs.Add(pf.Folder);
            //}

            //dirs.Sort();

            //foreach (var f in dirs)
            //{
            //    consoleController.WriteLine(f);
            //}

            //consoleController.ReadLine();
            //return;
            */

            #endregion

            #region GOG controllers

            IAuthorizationController authorizationController = new AuthorizationController(
                uriController,
                networkController,
                tokenExtractorController,
                loggingConsoleController);

            IFilterDelegate<Product> existingProductsFilter = new ExistingProductsFilter();

            IRequestDelegate<Product> pagedResultController =
                new MultipageRequestController(
                    networkController,
                    jsonStringController,
                    postUpdateDelegate,
                    existingProductsFilter);

            IProductCoreController<Product> productsController = new ProductsController(products);

            IGetStringDelegate gogDataController = new GOGDataController(networkController);

            ICollectionController<Product> ownedController = new OwnedController(owned);
            ICollectionController<long> updatedController = new UpdatedController(updated);

            IProductCoreController<ProductData> productsDataController = new ProductDataController(
                productsData,
                gogDataController,
                jsonStringController);

            IGameDetailsDownloadsController gameDetailsDownloadsController = new GameDetailsDownloadsController(
                languageCodesController);

            IProductCoreController<GameDetails> gamesDetailsController = new GameDetailsController(
                gamesDetails,
                networkController,
                jsonStringController,
                gameDetailsDownloadsController,
                settings.DownloadLanguageCodes);

            IFileValidationController fileValidationController = new FileValidationController(
                ioController,
                requestFileDelegate,
                postUpdateDelegate);

            IScreenshotsController screenshotsController = new ScreenshotsController(networkController);

            IFormattingController bytesFormattingController = new BytesFormattingController();
            IFormattingController secondsFormattingController = new SecondsFormattingController();

            IDownloadProgressReportingController downloadProgressReportingController = 
                new DownloadProgressReportingController(
                    bytesFormattingController, 
                    secondsFormattingController,
                    consoleController);

            var productFilesDownloadController = new ProductFilesDownloadController(
                requestFileDelegate,
                ioController,
                loggingConsoleController,
                downloadProgressReportingController);

            var imagesController = new ImagesController(
                requestFileDelegate,
                ioController,
                postUpdateDelegate);

            #endregion

            #region Authorize on site

            if (!authorizationController.Authorize(settings).Result)
            {
                consoleController.WriteLine("Press ENTER to quit...", ConsoleColor.White);
                consoleController.ReadLine();
                return;
            }

            #endregion

            #region Get new products from gog.com/games and gog.com/account

            loggingConsoleController.Write("Requesting new products from {0}...", ConsoleColor.White, Urls.GamesAjaxFiltered);

            var newProducts = pagedResultController.Request(
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                products).Result;

            if (newProducts != null)
            {
                loggingConsoleController.Write("Got {0} new products.", ConsoleColor.Gray, newProducts.Count);

                for (var pp = newProducts.Count - 1; pp >= 0; pp--)
                    productsController.Insert(0, newProducts[pp]);
            }

            saveLoadDataController.SaveData(products, ProductTypes.Products).Wait();

            consoleController.WriteLine(string.Empty, ConsoleColor.Gray);

            loggingConsoleController.Write("Requesting new products from {0}...", ConsoleColor.White, Urls.AccountGetFilteredProducts);

            var newOwned = pagedResultController.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                owned).Result;

            if (newOwned != null)
            {
                loggingConsoleController.Write("Got {0} new products.", ConsoleColor.Gray, newOwned.Count);

                for (var oo = newOwned.Count - 1; oo >= 0; oo--)
                {
                    ownedController.Insert(0, newOwned[oo]);

                    // also add to updated list as we haven't downloaded them previously,
                    // so they need to be updated just like other files
                    updatedController.Add(newOwned[oo].Id);
                }
            }

            saveLoadDataController.SaveData(owned, ProductTypes.Owned).Wait();

            loggingConsoleController.WriteLine(string.Empty, ConsoleColor.Gray);

            #endregion

            #region Get updated products

            loggingConsoleController.Write("Requesting updated products...", ConsoleColor.White);

            QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            var productUpdates = pagedResultController.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts).Result;

            if (productUpdates != null)
            {
                loggingConsoleController.Write("Got {0} updated products.", ConsoleColor.Gray, productUpdates.Count);

                foreach (var update in productUpdates)
                    updatedController.Add(update.Id);
            }

            saveLoadDataController.SaveData(updated, ProductTypes.Updated).Wait();

            loggingConsoleController.WriteLine(string.Empty, ConsoleColor.Gray);

            #endregion

            #region Get wishlisted products

            loggingConsoleController.Write("Requesting wishlisted products...", ConsoleColor.White);

            var wishlistedString = gogDataController.GetString(Urls.Wishlist).Result;
            var wishlistedProductResult = jsonStringController.Deserialize<ProductsResult>(wishlistedString);

            if (wishlistedProductResult != null &&
                wishlistedProductResult.Products != null)
            {
                var count = wishlistedProductResult.Products.Count;

                loggingConsoleController.Write("Got {0} wishlisted products.", ConsoleColor.Gray, count);

                wishlisted = new List<long>(count);

                foreach (var wish in wishlistedProductResult.Products)
                    wishlisted.Add(wish.Id);
            }

            saveLoadDataController.SaveData(wishlisted, ProductTypes.Wishlisted).Wait();

            consoleController.WriteLine(string.Empty, ConsoleColor.Gray);

            #endregion

            #region Update product data

            loggingConsoleController.Write("Updating product data...", ConsoleColor.White);

            productsDataController.OnProductUpdated += OnProductDataUpdated;

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
            saveLoadDataController.SaveData(productsData, ProductTypes.ProductsData).Wait();

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

            saveLoadDataController.SaveData(updated, ProductTypes.Updated).Wait();

            if (productsWithNewDLC.Count > 0)
            {
                productsDataController.Update(productsWithNewDLC, postUpdateDelegate).Wait();
                saveLoadDataController.SaveData(productsData, ProductTypes.ProductsData).Wait();
            }

            loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

            #endregion

            #region Enforce data consistency so that DLCs have proper ids

            loggingConsoleController.Write("Enforcing data consistency for DLCs...", ConsoleColor.White);

            var dataConsistencyController = new DataConsistencyController(gamesDetails, productsData);
            if (dataConsistencyController.Update())
            {
                // only save to disk if we've indeed updated gamesDetails
                // which shouldn't happen all updates but rather 
                // only when user obtains new product with owned DLC or just DLC
                saveLoadDataController.SaveData(gamesDetails, ProductTypes.GameDetails);
            }

            loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

            #endregion

            #region Extract screenshots uris from the product pages

            if (settings.DownloadScreenshots)
            {
                loggingConsoleController.Write("Requesting product screenshots...", ConsoleColor.White);

                foreach (var product in products)
                {
                    if (screenshots.ContainsKey(product.Id))
                    {

                        if (screenshots[product.Id] != null &&
                            screenshots[product.Id].Count > 0)
                            continue;
                        else
                        {
                            // we have the entry but it's null or empty - we'll try to get it again
                            screenshots.Remove(product.Id);
                        }
                    }

                    var productScreenshots = screenshotsController.GetScreenshotsUris(product, postUpdateDelegate).Result;

                    screenshots.Add(product.Id, productScreenshots);

                    saveLoadDataController.SaveData(screenshots, ProductTypes.Screenshots).Wait();
                }
            }

            loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

            #endregion

            #region Update images

            if (settings.DownloadImages)
            {
                loggingConsoleController.Write("Updating product images...", ConsoleColor.White);

                // for all products first
                imagesController.Update(products, imagesFolder).Wait();

                // then for owned products that are not in a products collection 
                // (e.g. no longer sold, part of bundle)
                imagesController.Update(existingProductsFilter.Filter(owned, products), imagesFolder).Wait();

                imagesController.Update(productsData, imagesFolder).Wait();

                // then screenshots
                if (settings.DownloadScreenshots)
                {
                    imagesController.Update(screenshots, screenshotsFolder).Wait();
                }

                loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);
            }

            #endregion

            #region Update products - game details, download and cleanup folders

            loggingConsoleController.WriteLine("Updating game details, product files and cleaning up product folders...", ConsoleColor.White);

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
                    loggingConsoleController.WriteLine("WARNING: Product {0} is not owned and couldn't be updated, removing it from updates.", ConsoleColor.Yellow, product.Title);
                    updated.Remove(u);
                    saveLoadDataController.SaveData(updated, ProductTypes.Updated).Wait();
                    continue;
                }

                // we use single loop to:
                // 1) update game details
                // 2) download updated product files
                // 3) validated downloaded product files
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
                            loggingConsoleController.WriteLine("Product {0} has been checked within last {1} day(s).",
                                ConsoleColor.DarkYellow,
                                u, dataCheckThresholdDays);
                            continue;
                        }
                    }
                }

                // request updated game details
                loggingConsoleController.Write("Updating game details for {0}...", ConsoleColor.White, u);

                gamesDetailsController.Update(new List<string> { u.ToString() }).Wait();

                // save new details
                saveLoadDataController.SaveData(gamesDetails, ProductTypes.GameDetails).Wait();

                loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

                // request updated product files
                var updatedGameDetails = gamesDetailsController.Find(u);

                IList<ProductFile> productIntallersExtras = new List<ProductFile>();

                if (settings.DownloadProductFiles)
                {
                    productIntallersExtras =
                        productFilesDownloadController.UpdateFiles(
                            updatedGameDetails,
                            settings.DownloadLanguageCodes,
                            settings.DownloadOperatingSystems).Result;
                }

                if (settings.ValidateProductFiles)
                {
                    foreach (var productFile in productIntallersExtras)
                    {
                        // reset validation status
                        productFile.Validated = false;

                        consoleController.Write("Validating {0}: {1}.", ConsoleColor.Gray, productFile.Name, productFile.File);

                        var result = fileValidationController.ValidateProductFile(productFile).Result;

                        if (!result.Item1) consoleController.WriteLine("ERROR: ", ConsoleColor.Red, result.Item2);
                        else
                        {
                            consoleController.WriteLine("Successfully validated.", ConsoleColor.Green);
                            productFile.Validated = true;
                        }
                    }

                    consoleController.WriteLine("DONE.", ConsoleColor.White);
                }

                IProductFileController productFilesController = new ProductFilesController(productIntallersExtras);

                // product files contain all files for that product, to store them:
                // - remove all entries for that id from productFiles
                productFiles = productFilesController.Filter(productFiles);
                // - add those new entries to productFiles
                foreach (var productFile in productIntallersExtras)
                    productFiles.Add(productFile);
                // - write updated product files
                saveLoadDataController.SaveData(productFiles, ProductTypes.ProductFiles);

                // remove from updated and cleanup only if all files were downloaded successfully
                if (productFilesController.CheckSuccess())
                {
                    // remove update entry as all files have been downloaded
                    if (updated.Contains(u))
                    {
                        updated.Remove(u);
                        saveLoadDataController.SaveData(updated, ProductTypes.Updated).Wait();
                    }

                    if (settings.CleanupProductFolders)
                    {
                        // cleanup product folder
                        loggingConsoleController.Write("Cleaning up product folder...", ConsoleColor.White);

                        ICleanupController cleanupController =
                            new CleanupController(
                                productFilesController,
                                fileValidationController,
                                ioController);

                        cleanupController.Cleanup(recycleBin);
                    }
                }
                else
                {
                    if (++failedAttempts >= failedAttemptThreshold)
                    {
                        loggingConsoleController.WriteLine("ERROR: Last {0} attempts to download product files were not successful. " +
                            "Recommended: wait 12-24 hours and try again. Abandoning attempts.", ConsoleColor.Red, failedAttempts);
                        break;
                    }
                }

                if (!checkedOwned.ContainsKey(u)) checkedOwned.Add(u, DateTime.Today);
                else checkedOwned[u] = DateTime.Today;

                saveLoadDataController.SaveData(checkedOwned, ProductTypes.CheckedOwned).Wait();

                loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

                // throttle server access
                if (settings.UpdateAll)
                {
                    loggingConsoleController.WriteLine("Waiting {0} minute(s) before next request...", 
                        ConsoleColor.Gray, 
                        updateAllThrottleMinutes);
                    System.Threading.Thread.Sleep(updateAllThrottleMilliseconds);
                }
            }

            loggingConsoleController.WriteLine("DONE.", ConsoleColor.White);

            #endregion

            #region Deauthorize on exit

            authorizationController.Deauthorize().Wait();

            #endregion

            loggingConsoleController.WriteLine("All done. Press ENTER to quit...", ConsoleColor.White);
            consoleController.ReadLine();

            #region Disposal of IO objects

            loggingConsoleController.Dispose();

            #endregion
        }
    }
}
