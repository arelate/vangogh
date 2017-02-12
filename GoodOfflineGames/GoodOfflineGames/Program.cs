using System;
using System.Collections.Generic;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Network;
using Controllers.Download;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.Extraction;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Settings;
using Controllers.RequestPage;
using Controllers.Throttle;
using Controllers.ImageUri;
using Controllers.Formatting;
using Controllers.LineBreaking;
using Controllers.UriResolution;
using Controllers.Destination.Directory;
using Controllers.Destination.Filename;
using Controllers.Cookies;
using Controllers.PropertiesValidation;
using Controllers.Validation;
using Controllers.Eligibility;
using Controllers.Conversion;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Indexing;
using Controllers.Measurement;
using Controllers.Presentation;
using Controllers.RecycleBin;
using Controllers.Routing;
using Controllers.TaskStatus;
using Controllers.Hash;

using Interfaces.ProductTypes;
using Interfaces.TaskActivity;

using GOG.Models;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;
using GOG.Controllers.Enumeration;

using GOG.TaskActivities.Authorization;
using GOG.TaskActivities.Load;
using GOG.TaskActivities.Update.PageResult;
using GOG.TaskActivities.Update.NewUpdatedAccountProducts;
using GOG.TaskActivities.Update.Wishlisted;
using GOG.TaskActivities.Update.Dependencies.Product;
using GOG.TaskActivities.Update.Dependencies.GameDetails;
using GOG.TaskActivities.Update.Dependencies.GameProductData;
using GOG.TaskActivities.Update.Products;
using GOG.TaskActivities.Update.Screenshots;
using GOG.TaskActivities.Download.Sources;
using GOG.TaskActivities.Download.ProductImages;
using GOG.TaskActivities.Download.Screenshots;
using GOG.TaskActivities.Download.ProductFiles;
using GOG.TaskActivities.Download.Validation;
using GOG.TaskActivities.Download.Processing;
using GOG.TaskActivities.Cleanup;

using GOG.TaskActivities.Validation;

using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;

using Models.TaskStatus;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Foundation Controllers

            var streamController = new StreamController();
            var fileController = new FileController();
            var directoryController = new DirectoryController();

            var storageController = new StorageController(
                streamController,
                fileController);
            var serializationController = new JSONStringController();

            var serializedStorageController = new SerializedStorageController(
                storageController,
                serializationController);

            var consoleController = new ConsoleController();
            var formattedStringMeasurementController = new FormattedStringMeasurementController();
            var lineBreakingController = new LineBreakingController(formattedStringMeasurementController);

            var presentationController = new PresentationController(
                formattedStringMeasurementController,
                lineBreakingController,
                consoleController);

            var bytesFormattingController = new BytesFormattingController();
            var secondsFormattingController = new SecondsFormattingController();

            var taskStatusTreeToListController = new TaskStatusTreeToListController();

            var applicationTaskStatus = new TaskStatus() { Title = "Welcome to GoodOfflineGames" };

            var taskStatusViewController = new TaskStatusViewController(
                applicationTaskStatus,
                bytesFormattingController,
                secondsFormattingController,
                taskStatusTreeToListController,
                presentationController);

            var taskStatusController = new TaskStatusController(taskStatusViewController);

            var cookiesController = new CookiesController(
                storageController,
                serializationController);

            var uriController = new UriController();
            var networkController = new NetworkController(
                cookiesController,
                uriController);

            var downloadController = new DownloadController(
                networkController,
                streamController,
                fileController,
                taskStatusController);

            var requestPageController = new RequestPageController(
                networkController);

            var languageController = new LanguageController();

            var loginTokenExtractionController = new LoginTokenExtractionController();
            var loginIdExtractionController = new LoginIdExtractionController();
            var loginUsernameExtractionController = new LoginUsernameExtractionController();

            var gogDataExtractionController = new GOGDataExtractionController();
            var screenshotExtractionController = new ScreenshotExtractionController();

            var collectionController = new CollectionController();

            var throttleController = new ThrottleController(
                taskStatusController,
                secondsFormattingController,
                200, // don't throttle if less than N items
                2 * 60); // 2 minutes

            var imageUriController = new ImageUriController();
            var screenshotUriController = new ScreenshotUriController();

            #endregion

            #region Data Controllers

            // Data controllers for products, game details, game product data, etc.

            var productCoreIndexingController = new ProductCoreIndexingController();

            // directories

            var dataDirectoryDelegate = new FixedDirectoryDelegate("data");

            var accountProductsDirectoryDelegate = new FixedDirectoryDelegate("accountProducts", dataDirectoryDelegate);
            var apiProductsDirectoryDelegate = new FixedDirectoryDelegate("apiProducts", dataDirectoryDelegate);
            var gameDetailsDirectoryDelegate = new FixedDirectoryDelegate("gameDetails", dataDirectoryDelegate);
            var gameProductDataDirectoryDelegate = new FixedDirectoryDelegate("gameProductData", dataDirectoryDelegate);
            var productsDirectoryDelegate = new FixedDirectoryDelegate("products", dataDirectoryDelegate);
            var productDownloadsDirectoryDelegate = new FixedDirectoryDelegate("productDownloads", dataDirectoryDelegate);
            var productRoutesDirectoryDelegate = new FixedDirectoryDelegate("productRoutes", dataDirectoryDelegate);
            var productScreenshotsDirectoryDelegate = new FixedDirectoryDelegate("productScreenshots", dataDirectoryDelegate);

            var recycleBinDirectoryDelegate = new FixedDirectoryDelegate("recycleBin");
            var imagesDirectoryDelegate = new FixedDirectoryDelegate("images");
            var logsDirectoryDelegate = new FixedDirectoryDelegate("logs");
            var validationDirectoryDelegate = new FixedDirectoryDelegate("md5");
            var productFilesBaseDirectoryDelegate = new FixedDirectoryDelegate("productFiles");
            var screenshotsDirectoryDelegate = new FixedDirectoryDelegate("screenshots");

            var productFilesDirectoryDelegate = new UriDirectoryDelegate(productFilesBaseDirectoryDelegate);

            // filenames

            var jsonFilenameDelegate = new JsonFilenameDelegate();

            var indexFilenameDelegate = new FixedFilenameDelegate("index", jsonFilenameDelegate);

            var wishlistedFilenameDelegate = new FixedFilenameDelegate("wishlisted", jsonFilenameDelegate);
            var updatedFilenameDelegate = new FixedFilenameDelegate("updated", jsonFilenameDelegate);
            var scheduledScreenshotsUpdatesFilenameDelegate = new FixedFilenameDelegate("scheduledScreenshotsUpdates", jsonFilenameDelegate);
            var scheduledCleanupFilenameDelegate = new FixedFilenameDelegate("scheduledCleanup", jsonFilenameDelegate);
            var scheduledRepairFilenameDelegate = new FixedFilenameDelegate("scheduledRepair", jsonFilenameDelegate);
            var lastKnownValidFilenameDelegate = new FixedFilenameDelegate("lastKnownValid", jsonFilenameDelegate);

            var uriFilenameDelegate = new UriFilenameDelegate();
            var logsFilenameDelegate = new LogFilenameDelegate();
            var validationFilenameDelegate = new ValidationFilenameDelegate();

            // index filenames

            var productsIndexDataController = new IndexDataController(
                collectionController,
                productsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var accountProductsIndexDataController = new IndexDataController(
                collectionController,
                accountProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var gameDetailsIndexDataController = new IndexDataController(
                collectionController,
                gameDetailsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var gameProductDataIndexDataController = new IndexDataController(
                collectionController,
                gameProductDataDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var apiProductsIndexDataController = new IndexDataController(
                collectionController,
                apiProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var productScreenshotsIndexDataController = new IndexDataController(
                collectionController,
                productScreenshotsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var productDownloadsIndexDataController = new IndexDataController(
                collectionController,
                productDownloadsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var productRoutesIndexDataController = new IndexDataController(
                collectionController,
                productRoutesDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            // index data controllers that are data controllers

            var wishlistedDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                wishlistedFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var updatedDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                updatedFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var scheduledScreenshotsUpdatesDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                scheduledScreenshotsUpdatesFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var lastKnownValidDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                lastKnownValidFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            var scheduledCleanupDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                scheduledCleanupFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            // data controllers

            var recycleBinController = new RecycleBinController(
                recycleBinDirectoryDelegate,
                fileController,
                directoryController);

            var productsDataController = new DataController<Product>(
                productsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var accountProductsDataController = new DataController<AccountProduct>(
                accountProductsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                accountProductsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var gameDetailsDataController = new DataController<GameDetails>(
                gameDetailsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                gameDetailsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var gameProductDataController = new DataController<GameProductData>(
                gameProductDataIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                gameProductDataDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var apiProductsDataController = new DataController<ApiProduct>(
                apiProductsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                apiProductsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                productScreenshotsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productScreenshotsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var productDownloadsDataController = new DataController<ProductDownloads>(
                productDownloadsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productDownloadsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            var productRoutesDataController = new DataController<ProductRoutes>(
                productRoutesIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productRoutesDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusController);

            #endregion

            #region Settings: Load, Validation

            var settingsController = new SettingsController(
                storageController,
                serializationController);

            var loadSettingsTask = taskStatusController.Create(applicationTaskStatus, "Load settings");
            var settings = settingsController.Load().Result;

            var validateSettingsTask = taskStatusController.Create(loadSettingsTask, "Validate settings");

            var validateDownloadSettingsTask = taskStatusController.Create(validateSettingsTask, "Validate download settings");
            var downloadPropertiesValidationController = new DownloadPropertiesValidationController(languageController);
            settings.Download = downloadPropertiesValidationController.ValidateProperties(settings.Download) as Models.Settings.DownloadProperties;
            taskStatusController.Complete(validateDownloadSettingsTask);

            taskStatusController.Complete(validateSettingsTask);
            taskStatusController.Complete(loadSettingsTask);

            // set user agent string used for network requests
            if (!string.IsNullOrEmpty(settings.Connection.UserAgent))
                networkController.UserAgent = settings.Connection.UserAgent;

            #endregion

            #region Task Activity Controllers

            #region Load

            var loadDataController = new LoadDataController(
                applicationTaskStatus,
                taskStatusController,
                productsDataController,
                accountProductsDataController,
                gameDetailsDataController,
                gameProductDataController,
                screenshotsDataController,
                apiProductsDataController,
                wishlistedDataController,
                updatedDataController,
                scheduledScreenshotsUpdatesDataController,
                productDownloadsDataController,
                productRoutesDataController,
                lastKnownValidDataController,
                scheduledCleanupDataController);

            #endregion

            #region Authorization

            var authenticationPropertiesValidationController = new AuthenticationPropertiesValidationController(consoleController);

            var authorizationController = new AuthorizationController(
                uriController,
                networkController,
                serializationController,
                loginTokenExtractionController,
                loginIdExtractionController,
                loginUsernameExtractionController,
                consoleController,
                settings.Authentication,
                authenticationPropertiesValidationController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #region Update.PageResults

            var productsPageResultsController = new ProductsPageResultController(
                ProductTypes.Product,
                requestPageController,
                serializationController,
                taskStatusController);

            var productsExtractionController = new ProductsExtractionController();

            var productsUpdateController = new PageResultUpdateController<
                ProductsPageResult,
                Product>(
                    ProductTypes.Product,
                    productsPageResultsController,
                    productsExtractionController,
                    requestPageController,
                    productsDataController,
                    applicationTaskStatus,
                    taskStatusController);

            var accountProductsPageResultsController = new AccountProductsPageResultController(
                ProductTypes.AccountProduct,
                requestPageController,
                serializationController,
                taskStatusController);

            var accountProductsExtractionController = new AccountProductsExtractionController();

            var accountProductsUpdateController = new PageResultUpdateController<
                AccountProductsPageResult,
                AccountProduct>(
                    ProductTypes.AccountProduct,
                    accountProductsPageResultsController,
                    accountProductsExtractionController,
                    requestPageController,
                    accountProductsDataController,
                    applicationTaskStatus,
                    taskStatusController);

            #endregion

            #region Update.NewUpdatedAccountProducts

            var newUpdatedAccountProductsController = new NewUpdatedAccountProductsController(
                updatedDataController,
                lastKnownValidDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #region Update.Wishlisted

            var wishlistedUpdateController = new WishlistedUpdateController(
                networkController,
                gogDataExtractionController,
                serializationController,
                wishlistedDataController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var productUpdateUriController = new ProductUpdateUriController();

            var gameProductDataUpdateUriController = new GameProductDataUpdateUriController();
            var gameProductDataSkipUpdateController = new GameProductDataSkipUpdateController(
                productsDataController);
            var gameProductDataDecodingController = new GameProductDataDecodingController(
                gogDataExtractionController,
                serializationController);

            var gameDetailsRequiredUpdatesController = new GameDetailsRequiredUpdatesController(
                updatedDataController);
            var gameDetailsConnectionController = new GameDetailsConnectionController();
            var gameDetailsDownloadDetailsController = new GameDetailsDownloadDetailsController(
                serializationController,
                languageController);

            // product update controllers

            var gameProductDataUpdateController = new GameProductDataUpdateController(
                gameProductDataController,
                productsDataController,
                networkController,
                serializationController,
                gameProductDataUpdateUriController,
                gameProductDataSkipUpdateController,
                gameProductDataDecodingController,
                applicationTaskStatus,
                taskStatusController);

            var apiProductUpdateController = new ApiProductUpdateController(
                apiProductsDataController,
                productsDataController,
                networkController,
                serializationController,
                productUpdateUriController,
                applicationTaskStatus,
                taskStatusController);

            var gameDetailsUpdateController = new GameDetailsUpdateController(
                gameDetailsDataController,
                accountProductsDataController,
                networkController,
                serializationController,
                throttleController,
                productUpdateUriController,
                gameDetailsRequiredUpdatesController,
                gameDetailsConnectionController,
                gameDetailsDownloadDetailsController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #region Update.Screenshots

            var screenshotUpdateController = new ScreenshotUpdateController(
                screenshotsDataController,
                scheduledScreenshotsUpdatesDataController,
                productsDataController,
                networkController,
                screenshotExtractionController,
                applicationTaskStatus,
                taskStatusController);

            #endregion



            // dependencies for download controllers

            var productsImagesDownloadSourcesController = new ProductsImagesDownloadSourcesController(
                updatedDataController,
                productsDataController,
                imageUriController);

            var accountProductsImagesDownloadSourcesController = new AccountProductsImagesDownloadSourcesController(
                updatedDataController,
                accountProductsDataController,
                imageUriController);

            var screenshotsDownloadSourcesController = new ScreenshotsDownloadSourcesController(
                scheduledScreenshotsUpdatesDataController,
                screenshotsDataController,
                screenshotUriController,
                taskStatusController);

            var routingController = new RoutingController(productRoutesDataController);

            var gameDetailsManualUrlsEnumerationController = new GameDetailsManualUrlEnumerationController(
                settings.Download.Languages,
                settings.Download.OperatingSystems,
                gameDetailsDataController);

            var gameDetailsDirectoryEnumerationController = new GameDetailsDirectoryEnumerationController(
                settings.Download.Languages,
                settings.Download.OperatingSystems,
                gameDetailsDataController,
                productFilesDirectoryDelegate);

            var gameDetailsFilesEnumerationController = new GameDetailsFileEnumerationController(
                settings.Download.Languages,
                settings.Download.OperatingSystems,
                gameDetailsDataController,
                routingController,
                productFilesDirectoryDelegate,
                uriFilenameDelegate);

            var productFilesDownloadSourcesController = new ProductFilesDownloadSourcesController(
                updatedDataController,
                gameDetailsManualUrlsEnumerationController);

            var validationUriResolutionController = new ValidationUriResolutionController();

            var downloadEntryValidationEligibilityController = new DownloadEntryValidationEligibilityController();
            var updateRouteEligibilityController = new UpdateRouteEligibilityController();
            var removeEntryEligibilityController = new RemoveEntryEligibilityController();
            var fileValidationEligibilityController = new FileValidationEligibilityController();

            var validationDownloadSourcesController = new ValidationDownloadSourcesController(
                updatedDataController,
                productDownloadsDataController,
                routingController,
                validationUriResolutionController,
                downloadEntryValidationEligibilityController,
                fileValidationEligibilityController);

            // schedule download controllers

            var updateProductsImagesDownloadsController = new UpdateImagesDownloadsController(
                ProductDownloadTypes.Image,
                productsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var updateAccountProductsImagesDownloadsController = new UpdateImagesDownloadsController(
                ProductDownloadTypes.Image,
                accountProductsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var updateScreenshotsDownloadsController = new UpdateScreenshotsDownloadsController(
                ProductDownloadTypes.Screenshot,
                screenshotsDownloadSourcesController,
                screenshotsDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var updateProductFilesDownloadsController = new UpdateFilesDownloadsController(
                ProductDownloadTypes.ProductFile,
                productFilesDownloadSourcesController,
                productFilesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            // downloads processing

            var imagesProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.Image,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            var screenshotsProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.Screenshot,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            var productFilesProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.ProductFile,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            // validation controllers

            var updateValidationDownloadsController = new UpdateValidationDownloadsController(
                ProductDownloadTypes.Validation,
                validationDownloadSourcesController,
                validationDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var validationProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.Validation,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            var bytesToStringConversionController = new BytesToStringConvertionController();

            var md5HashController = new BytesToStringMd5HashController(bytesToStringConversionController);

            var validationController = new ValidationController(
                validationDirectoryDelegate,
                validationFilenameDelegate,
                fileController,
                streamController,
                md5HashController,
                taskStatusController);

            var processValidationController = new ProcessValidationController(
                productFilesDirectoryDelegate,
                uriFilenameDelegate,
                validationController,
                productDownloadsDataController,
                updatedDataController,
                lastKnownValidDataController,
                scheduledCleanupDataController,
                routingController,
                downloadEntryValidationEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            #region Cleanup

            var directoryCleanupController = new DirectoryCleanupController(
                gameDetailsDataController,
                gameDetailsDirectoryEnumerationController,
                productFilesDirectoryDelegate,
                directoryController,
                recycleBinController,
                applicationTaskStatus,
                taskStatusController);

            var filesCleanupController = new FilesCleanupController(
                scheduledCleanupDataController,
                accountProductsDataController,
                gameDetailsFilesEnumerationController,
                gameDetailsDirectoryEnumerationController,
                directoryController,
                fileValidationEligibilityController,
                validationDirectoryDelegate,
                uriFilenameDelegate,
                recycleBinController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #endregion

            #region TACs Execution

            #region Initialization Task Activities (always performed)

            var taskActivityControllers = new List<ITaskActivityController>
            {
                // load initial data
                loadDataController,
                // authorize
                authorizationController
            };

            #endregion

            #region Data Updates Task Activities

            // data updates
            if (settings.Update.Products)
                taskActivityControllers.Add(productsUpdateController);
            if (settings.Update.AccountProducts)
                taskActivityControllers.Add(accountProductsUpdateController);
            if (settings.Update.NewUpdatedAccountProducts)
                taskActivityControllers.Add(newUpdatedAccountProductsController);
            if (settings.Update.Wishlist)
                taskActivityControllers.Add(wishlistedUpdateController);

            // product/account product dependent data updates
            if (settings.Update.GameProductData)
                taskActivityControllers.Add(gameProductDataUpdateController);
            if (settings.Update.ApiProducts)
                taskActivityControllers.Add(apiProductUpdateController);
            if (settings.Update.GameDetails)
                taskActivityControllers.Add(gameDetailsUpdateController);
            if (settings.Update.Screenshots)
                taskActivityControllers.Add(screenshotUpdateController);

            #endregion

            #region Download Task Activities

            // schedule downloads
            if (settings.Download.ProductsImages)
                taskActivityControllers.Add(updateProductsImagesDownloadsController);
            if (settings.Download.AccountProductsImages)
                taskActivityControllers.Add(updateAccountProductsImagesDownloadsController);
            if (settings.Download.Screenshots)
                taskActivityControllers.Add(updateScreenshotsDownloadsController);
            if (settings.Download.ProductsFiles)
                taskActivityControllers.Add(updateProductFilesDownloadsController);

            //actually download images, screenshots, product files, extras
            if (settings.Download.ProductsImages ||
                settings.Download.AccountProductsImages)
                taskActivityControllers.Add(imagesProcessScheduledDownloadsController);
            if (settings.Download.Screenshots)
                taskActivityControllers.Add(screenshotsProcessScheduledDownloadsController);
            if (settings.Download.ProductsFiles)
                taskActivityControllers.Add(productFilesProcessScheduledDownloadsController);

            #endregion

            #region Validation Task Activities

            // validation downloads should follow productFiles download processing, because they use timed CDN key
            if (settings.Validation.Download)
                taskActivityControllers.Add(updateValidationDownloadsController);
            if (settings.Validation.Download)
                taskActivityControllers.Add(validationProcessScheduledDownloadsController);
            if (settings.Validation.ValidateUpdated)
                taskActivityControllers.Add(processValidationController);

            #endregion

            #region Cleanup Task Activities

            // cleanup directories
            if (settings.Cleanup.Directories)
                taskActivityControllers.Add(directoryCleanupController);
            // cleanup files
            if (settings.Cleanup.Files)
                taskActivityControllers.Add(filesCleanupController);

            #endregion

            foreach (var taskActivityController in taskActivityControllers)
            {
                try
                {
                    taskActivityController.ProcessTaskAsync().Wait();
                    taskStatusViewController.CreateView(true);
                }
                catch (AggregateException ex)
                {
                    List<string> errorMessages = new List<string>();

                    foreach (var innerException in ex.InnerExceptions)
                        errorMessages.Add(innerException.Message);

                    taskStatusController.Fail(applicationTaskStatus, string.Join(", ", errorMessages));
                    break;
                }
            }

            taskStatusController.Complete(applicationTaskStatus);
            taskStatusViewController.CreateView(true);

            #region Save log 

            if (settings.Log)
            {
                var uri = System.IO.Path.Combine(
                    logsDirectoryDelegate.GetDirectory(),
                    logsFilenameDelegate.GetFilename());

                presentationController.Present(new List<Tuple<string, string[]>>
                {
                    Tuple.Create(string.Format("Save log to {0}", uri), new string[] { "white" })
                });

                serializedStorageController.SerializePushAsync(uri, applicationTaskStatus).Wait();
            }

            #endregion

            var defaultColor = new string[] { " default" };

            presentationController.Present(
                new List<Tuple<string, string[]>> {
                    Tuple.Create("%cAll GoodOfflineGames tasks are complete.", new string[] { "white" }),
                    Tuple.Create("", defaultColor),
                    Tuple.Create("%cPress ENTER to close the window...", defaultColor)
                }, true);
            consoleController.ReadLine();

            #endregion
        }
    }
}
