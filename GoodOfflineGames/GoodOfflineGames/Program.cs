#region Using

using System;
using System.Collections.Generic;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Uri;
using Controllers.Network;
using Controllers.FileDownload;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.Extraction;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Settings;
using Controllers.ActivityParameters;
using Controllers.RequestPage;
using Controllers.Throttle;
using Controllers.ImageUri;
using Controllers.Formatting;
using Controllers.LineBreaking;
using Controllers.Destination.Directory;
using Controllers.Destination.Filename;
using Controllers.Cookies;
using Controllers.PropertiesValidation;
using Controllers.Validation;
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
using Controllers.Containment;
using Controllers.Sanitization;
using Controllers.Session;
using Controllers.Expectation;
using Controllers.UpdateUri;
using Controllers.Naming;

using Interfaces.ProductTypes;
using Interfaces.TaskActivity;

using GOG.Models;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;
using GOG.Controllers.Enumeration;
using GOG.Controllers.Network;
using GOG.Controllers.Connection;
using GOG.Controllers.UpdateUri;
using GOG.Controllers.FileDownload;
using GOG.Controllers.Collection;
using GOG.Controllers.DownloadSources;
using GOG.Controllers.Authorization;

using GOG.TaskActivities.Load;
using GOG.TaskActivities.ValidateSettings;
using GOG.TaskActivities.Authorize;
using GOG.TaskActivities.UpdateData;
using GOG.TaskActivities.UpdateDownloads;
using GOG.TaskActivities.ProcessDownloads;
using GOG.TaskActivities.Cleanup;
using GOG.TaskActivities.Validate;
using GOG.TaskActivities.LogTaskStatus;
using GOG.TaskActivities.ActivityParameters;

using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;

using Models.TaskStatus;

#endregion

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

            var jsonFilenameDelegate = new JsonFilenameDelegate();
            var uriHashesFilenameDelegate = new FixedFilenameDelegate("hashes", jsonFilenameDelegate);

            var hashTrackingController = new HashTrackingController(
                uriHashesFilenameDelegate,
                serializationController,
                storageController);

            var bytesToStringConversionController = new BytesToStringConvertionController();
            var md5HashController = new BytesToStringMd5HashController(bytesToStringConversionController);

            var serializedStorageController = new SerializedStorageController(
                hashTrackingController,
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

            var fileDownloadController = new FileDownloadController(
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

            var settingsFilenameDelegate = new FixedFilenameDelegate("settings", jsonFilenameDelegate);

            var settingsController = new SettingsController(
                settingsFilenameDelegate,
                serializedStorageController);

            var downloadsLanguagesValidationDelegate = new DownloadsLanguagesValidationDelegate(languageController);
            var downloadsOperatingSystemsValidationDelegate = new DownloadsOperatingSystemsValidationDelegate();

            var validateSettingsTaskActivity = new ValidateSettingsController(
                settingsController,
                downloadsLanguagesValidationDelegate,
                downloadsOperatingSystemsValidationDelegate,
                taskStatusController);

            #endregion

            #region Activity Parameters 

            var activityParametersFilenameDelegate = new FixedFilenameDelegate("activityParameters", jsonFilenameDelegate);

            var activityParametersController = new ActivityParametersController(
                activityParametersFilenameDelegate,
                serializedStorageController);

            #endregion

            #region Task Activity Controllers

            #region Load

            var loadDataController = new LoadDataController(
                taskStatusController,
                settingsController,
                activityParametersController,
                hashTrackingController,
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

            var usernamePasswordValidationDelegate = new UsernamePasswordValidationDelegate(consoleController);

            var authorizationController = new AuthorizationController(
                usernamePasswordValidationDelegate,
                uriController,
                networkController,
                serializationController,
                loginTokenExtractionController,
                loginIdExtractionController,
                loginUsernameExtractionController,
                consoleController);

            var authorizeController = new AuthorizeController(
                settingsController,
                authorizationController,
                taskStatusController);

            #endregion

            #region Update.PageResults

            var productTypesGetUpdateUriDelegate = new ProductTypesGetUpdateUriDelegate();

            var productsPageResultsController = new PageResultsController<ProductsPageResult>(
                ProductTypes.Product,
                productTypesGetUpdateUriDelegate,
                requestPageController,
                hashTrackingController,
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
                    taskStatusController);

            var accountProductsPageResultsController = new PageResultsController<AccountProductsPageResult>(
                ProductTypes.AccountProduct,
                productTypesGetUpdateUriDelegate,
                requestPageController,
                hashTrackingController,
                serializationController,
                taskStatusController);

            var accountProductsExtractionController = new AccountProductsExtractionController();

            var newUpdatedCollectionProcessingController = new NewUpdatedCollectionProcessingController(
                collectionController,
                updatedDataController,
                lastKnownValidDataController,
                taskStatusController);

            var accountProductsUpdateController = new PageResultUpdateController<
                AccountProductsPageResult,
                AccountProduct>(
                    ProductTypes.AccountProduct,
                    accountProductsPageResultsController,
                    accountProductsExtractionController,
                    requestPageController,
                    accountProductsDataController,
                    taskStatusController,
                    newUpdatedCollectionProcessingController);

            #endregion

            #region Update.Wishlisted

            var getProductsPageResultDelegate = new GetDeserializedGOGDataDelegate<ProductsPageResult>(networkController,
                gogDataExtractionController,
                serializationController);

            var wishlistedUpdateController = new WishlistedUpdateController(
                getProductsPageResultDelegate,
                wishlistedDataController,
                taskStatusController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var getGOGDataDelegate = new GetDeserializedGOGDataDelegate<GOGData>(networkController,
                gogDataExtractionController,
                serializationController);

            var getGameProductDataDeserializedDelegate = new GetGameProductDataDeserializedDelegate(
                getGOGDataDelegate);

            var productIdUpdateUriDelegate = new ProductIdUpdateUriDelegate();
            var productUrlUpdateUriDelegate = new ProductUrlUpdateUriDelegate();
            var accountProductIdUpdateUriDelegate = new AccountProductIdUpdateUriDelegate();

            var gameDetailsAccountProductConnectDelegate = new GameDetailsAccountProductConnectDelegate();

            // product update controllers

            var gameProductDataUpdateController = new ProductCoreUpdateController<GameProductData, Product>(
                ProductTypes.GameProductData,
                productTypesGetUpdateUriDelegate,
                gameProductDataController,
                productsDataController,
                updatedDataController,
                getGameProductDataDeserializedDelegate,
                productUrlUpdateUriDelegate,
                taskStatusController);

            var getApriProductDelegate = new GetDeserializedGOGModelDelegate<ApiProduct>(
                networkController,
                serializationController);

            var apiProductUpdateController = new ProductCoreUpdateController<ApiProduct, Product>(
                ProductTypes.AccountProduct,
                productTypesGetUpdateUriDelegate,
                apiProductsDataController,
                productsDataController,
                updatedDataController,
                getApriProductDelegate,
                productIdUpdateUriDelegate,
                taskStatusController);

            var getDeserializedGameDetailsDelegate = new GetDeserializedGOGModelDelegate<GameDetails>(
                networkController,
                serializationController);

            var languageDownloadsContainmentController = new StringContainmentController(
                collectionController,
                Models.Separators.Separators.GameDetailsDownloadsStart,
                Models.Separators.Separators.GameDetailsDownloadsEnd);

            var gameDetailsLanguagesExtractionController = new GameDetailsLanguagesExtractionController();
            var gameDetailsDownloadsExtractionController = new GameDetailsDownloadsExtractionController();

            var sanitizationController = new SanitizationController();

            var operatingSystemsDownloadsExtractionController = new OperatingSystemsDownloadsExtractionController(
                sanitizationController,
                languageController);

            var getGameDetailsDelegate = new GetDeserializedGameDetailsDelegate(
                networkController,
                serializationController,
                languageController,
                languageDownloadsContainmentController,
                gameDetailsLanguagesExtractionController,
                gameDetailsDownloadsExtractionController,
                sanitizationController,
                operatingSystemsDownloadsExtractionController);

            var gameDetailsUpdateController = new ProductCoreUpdateController<GameDetails, AccountProduct>(
                ProductTypes.GameDetails,
                productTypesGetUpdateUriDelegate,
                gameDetailsDataController,
                accountProductsDataController,
                updatedDataController,
                getGameDetailsDelegate,
                accountProductIdUpdateUriDelegate,
                taskStatusController,
                throttleController,
                gameDetailsAccountProductConnectDelegate);

            #endregion

            #region Update.Screenshots

            var screenshotUpdateController = new ScreenshotUpdateController(
                productTypesGetUpdateUriDelegate,
                screenshotsDataController,
                scheduledScreenshotsUpdatesDataController,
                productsDataController,
                networkController,
                screenshotExtractionController,
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
                settingsController,
                gameDetailsDataController);

            var gameDetailsDirectoryEnumerationController = new GameDetailsDirectoryEnumerationController(
                settingsController,
                gameDetailsDataController,
                productFilesDirectoryDelegate);

            var gameDetailsFilesEnumerationController = new GameDetailsFileEnumerationController(
                settingsController,
                gameDetailsDataController,
                routingController,
                productFilesDirectoryDelegate,
                uriFilenameDelegate);

            // product files and validation files are driven through gameDetails manual urls
            // so this sources enumerates all manual urls for all updated game details
            var manualUrlDownloadSourcesController = new ManualUrlDownloadSourcesController(
                updatedDataController,
                gameDetailsManualUrlsEnumerationController);

            // schedule download controllers

            var updateProductsImagesDownloadsController = new UpdateDownloadsController(
                ProductDownloadTypes.Image,
                productsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatusController);

            var updateAccountProductsImagesDownloadsController = new UpdateDownloadsController(
                ProductDownloadTypes.Image,
                accountProductsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatusController);

            var updateScreenshotsDownloadsController = new UpdateDownloadsController(
                ProductDownloadTypes.Screenshot,
                screenshotsDownloadSourcesController,
                screenshotsDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatusController);

            var updateProductFilesDownloadsController = new UpdateDownloadsController(
                ProductDownloadTypes.ProductFile,
                manualUrlDownloadSourcesController,
                productFilesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatusController);

            // downloads processing

            var imagesProcessScheduledDownloadsController = new ProcessDownloadsController(
                ProductDownloadTypes.Image,
                updatedDataController,
                productDownloadsDataController,
                fileDownloadController,
                taskStatusController);

            var screenshotsProcessScheduledDownloadsController = new ProcessDownloadsController(
                ProductDownloadTypes.Screenshot,
                updatedDataController,
                productDownloadsDataController,
                fileDownloadController,
                taskStatusController);

            var sesionUriExtractionController = new SessionUriExtractionController();
            var sessionController = new SessionController(
                networkController,
                sesionUriExtractionController);

            var manualUrlDownloadFromSourceDelegate = new ManualUrlDownloadFromSourceDelegate(
                networkController,
                sessionController,
                routingController,
                fileDownloadController,
                taskStatusController);

            var productFilesProcessScheduledDownloadsController = new ProcessDownloadsController(
                ProductDownloadTypes.ProductFile,
                updatedDataController,
                productDownloadsDataController,
                manualUrlDownloadFromSourceDelegate,
                taskStatusController);

            // validation controllers

            var updateValidationDownloadsController = new UpdateDownloadsController(
                ProductDownloadTypes.Validation,
                manualUrlDownloadSourcesController,
                validationDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatusController);

            var validationExpectedDelegate = new ValidationExpectedDelegate();
            var validationUriController = new ValidationUriController(uriController);

            var validationDownloadFromSourceDelegate = new ValidationDownloadFromSourceDelegate(
                routingController,
                sessionController,
                validationExpectedDelegate,
                validationUriController,
                fileDownloadController,
                taskStatusController);

            var validationProcessScheduledDownloadsController = new ProcessDownloadsController(
                ProductDownloadTypes.Validation,
                updatedDataController,
                productDownloadsDataController,
                validationDownloadFromSourceDelegate,
                taskStatusController);

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
                gameDetailsManualUrlsEnumerationController,
                validationExpectedDelegate,
                updatedDataController,
                lastKnownValidDataController,
                scheduledCleanupDataController,
                routingController,
                taskStatusController);

            #region Cleanup

            var directoryCleanupController = new DirectoryCleanupController(
                gameDetailsDataController,
                gameDetailsDirectoryEnumerationController,
                productFilesDirectoryDelegate,
                directoryController,
                recycleBinController,
                taskStatusController);

            var filesCleanupController = new FilesCleanupController(
                scheduledCleanupDataController,
                accountProductsDataController,
                gameDetailsFilesEnumerationController,
                gameDetailsDirectoryEnumerationController,
                directoryController,
                validationDirectoryDelegate,
                uriFilenameDelegate,
                recycleBinController,
                taskStatusController);

            #endregion

            #region Log Task Status 

            var logTaskStatusController = new LogTaskStatusController(
                logsDirectoryDelegate,
                logsFilenameDelegate,
                serializedStorageController,
                taskStatusController);

            #endregion

            #endregion

            #region Task Activities Parameters

            var activityParametersNameDelegate = new ActivityParametersNameDelegate();

            // convert strings to controller that concatenates
            var activityParametersTaskActivities = new Dictionary<string, ITaskActivityController>()
            {
                { activityParametersNameDelegate.GetName("updateData", "products"),
                    productsUpdateController },
                { activityParametersNameDelegate.GetName("updateData", "accountProducts"),
                    accountProductsUpdateController },
                { activityParametersNameDelegate.GetName("updateData","wishlist"),
                    wishlistedUpdateController },
                { activityParametersNameDelegate.GetName("updateData","gameProductData"),
                    gameProductDataUpdateController },
                { activityParametersNameDelegate.GetName("updateData","apiProducts"),
                    apiProductUpdateController },
                { activityParametersNameDelegate.GetName("updateData","gameDetails"),
                    gameDetailsUpdateController },
                { activityParametersNameDelegate.GetName("updateData","screenshots"),
                    screenshotUpdateController },
                { activityParametersNameDelegate.GetName("updateDownloads","productsImages"),
                    updateProductsImagesDownloadsController },
                { activityParametersNameDelegate.GetName("updateDownloads","accountProductsImages"),
                    updateAccountProductsImagesDownloadsController },
                { activityParametersNameDelegate.GetName("updateDownloads","screenshots"),
                    updateScreenshotsDownloadsController },
                { activityParametersNameDelegate.GetName("updateDownloads","productsFiles"),
                    updateProductFilesDownloadsController },
                { activityParametersNameDelegate.GetName("updateDownloads","validationFiles"),
                    updateValidationDownloadsController },
                { activityParametersNameDelegate.GetName("processDownloads","productsImages"),
                    imagesProcessScheduledDownloadsController },
                { activityParametersNameDelegate.GetName("processDownloads","accountProductsImages"),
                    imagesProcessScheduledDownloadsController },
                { activityParametersNameDelegate.GetName("processDownloads","screenshots"),
                    screenshotsProcessScheduledDownloadsController },
                { activityParametersNameDelegate.GetName("processDownloads","productsFiles"),
                    productFilesProcessScheduledDownloadsController },
                { activityParametersNameDelegate.GetName("processDownloads","validationFiles"),
                    validationProcessScheduledDownloadsController },
                { activityParametersNameDelegate.GetName("validate","productFiles"),
                    processValidationController },
                { activityParametersNameDelegate.GetName("cleanup","directories"),
                    directoryCleanupController },
                { activityParametersNameDelegate.GetName("cleanup","files"),
                    filesCleanupController },
                { activityParametersNameDelegate.GetName("logTaskStatus","true"),
                    logTaskStatusController }
            };

            var processActivityParametersController = new ProcessActivityParametersController(
                activityParametersController,
                activityParametersNameDelegate,
                activityParametersTaskActivities,
                taskStatusController);

            #endregion

            #region Initialization Task Activities (always performed)

            var taskActivityControllers = new List<ITaskActivityController>
            {
                // load initial data
                loadDataController,
                // validate settings
                validateSettingsTaskActivity,
                // authorize
                authorizeController,
                //  activity parameters
                processActivityParametersController
            };

            #endregion

            foreach (var taskActivityController in taskActivityControllers)
            {
                try
                {
                    taskActivityController.ProcessTaskAsync(applicationTaskStatus).Wait();
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

            var defaultColor = new string[] { " default" };

            presentationController.Present(
                new List<Tuple<string, string[]>> {
                    Tuple.Create("%cAll GoodOfflineGames tasks are complete.", new string[] { "white" }),
                    Tuple.Create("", defaultColor),
                    Tuple.Create("%cPress ENTER to close the window...", defaultColor)
                }, true);
            consoleController.ReadLine();
        }
    }
}
