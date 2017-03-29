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
using Controllers.RequestRate;
using Controllers.ImageUri;
using Controllers.Formatting;
using Controllers.LineBreaking;
using Controllers.Destination.Directory;
using Controllers.Destination.Filename;
using Controllers.Destination.Uri;
using Controllers.Cookies;
using Controllers.PropertiesValidation;
using Controllers.Validation;
using Controllers.Conversion;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Indexing;
using Controllers.Presentation;
using Controllers.RecycleBin;
using Controllers.Routing;
using Controllers.TaskStatus;
using Controllers.Hash;
using Controllers.Containment;
using Controllers.Sanitization;
using Controllers.Expectation;
using Controllers.UpdateUri;
using Controllers.Naming;
using Controllers.QueryParameters;
using Controllers.Template;
using Controllers.ViewModel;
using Controllers.Tree;
using Controllers.ViewController;
using Controllers.Enumeration;

using Interfaces.TaskActivity;

using GOG.Models;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;
using GOG.Controllers.Enumeration;
using GOG.Controllers.Network;
using GOG.Controllers.Connection;
using GOG.Controllers.UpdateIdentity;
using GOG.Controllers.FileDownload;
using GOG.Controllers.DataRefinement;
using GOG.Controllers.DownloadSources;
using GOG.Controllers.Authorization;

using GOG.TaskActivities.Load;
using GOG.TaskActivities.ValidateSettings;
using GOG.TaskActivities.Authorize;
using GOG.TaskActivities.ActivityParameters;
using GOG.TaskActivities.UpdateData;
using GOG.TaskActivities.UpdateDownloads;
using GOG.TaskActivities.Download;
using GOG.TaskActivities.Cleanup;
using GOG.TaskActivities.Validate;
using GOG.TaskActivities.LogTaskStatus;

using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;

using Models.TaskStatus;
using Models.ActivityParameters;

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
            var lineBreakingController = new LineBreakingController();

            var consolePresentationController = new ConsolePresentationController(
                lineBreakingController,
                consoleController);

            var bytesFormattingController = new BytesFormattingController();
            var secondsFormattingController = new SecondsFormattingController();

            var collectionController = new CollectionController();

            var taskStatusTreeToEnumerableController = new TaskStatusTreeToEnumerableController();

            var applicationTaskStatus = new TaskStatus() { Title = "Welcome to GoodOfflineGames" };

            var templatesDirectoryDelegate = new FixedDirectoryDelegate("templates");
            var appTemplateFilenameDelegate = new FixedFilenameDelegate("app", jsonFilenameDelegate);
            var reportTemplateFilenameDelegate = new FixedFilenameDelegate("report", jsonFilenameDelegate);

            var appTemplateController = new TemplateController(
                "taskStatus",
                templatesDirectoryDelegate,
                appTemplateFilenameDelegate,
                serializedStorageController,
                collectionController);

            var reportTemplateController = new TemplateController(
                "taskStatus",
                templatesDirectoryDelegate,
                reportTemplateFilenameDelegate,
                serializedStorageController,
                collectionController);

            var taskStatusAppViewModelDelegate = new TaskStatusAppViewModelDelegate(
                bytesFormattingController,
                secondsFormattingController);

            var taskStatusReportViewModelDelegate = new TaskStatusReportViewModelDelegate(
                bytesFormattingController,
                secondsFormattingController);

            var taskStatusAppViewController = new TaskStatusViewController(
                applicationTaskStatus,
                appTemplateController,
                taskStatusAppViewModelDelegate,
                taskStatusTreeToEnumerableController,
                consolePresentationController);

            var taskStatusAppController = new TaskStatusController(taskStatusAppViewController);

            var cookiesController = new CookiesController(
                storageController,
                serializationController);

            var throttleController = new ThrottleController(
                taskStatusAppController,
                secondsFormattingController);

            var requestRateController = new RequestRateController(
                throttleController,
                collectionController,
                taskStatusAppController,
                new string[] {
                    Models.Uris.Uris.Paths.Account.GameDetails, // gameDetails requests
                    Models.Uris.Uris.Paths.ProductFiles.ManualUrlDownlink, // manualUrls from gameDetails requests
                    Models.Uris.Uris.Paths.ProductFiles.ManualUrlCDNSecure // resolved manualUrls and validation files requests
                });

            var uriController = new UriController();
            var networkController = new NetworkController(
                cookiesController,
                uriController,
                requestRateController);

            var fileDownloadController = new FileDownloadController(
                networkController,
                streamController,
                fileController,
                taskStatusAppController);

            var requestPageController = new RequestPageController(
                networkController);

            var languageController = new LanguageController();

            var loginTokenExtractionController = new LoginTokenExtractionController();
            var loginIdExtractionController = new LoginIdExtractionController();
            var loginUsernameExtractionController = new LoginUsernameExtractionController();

            var gogDataExtractionController = new GOGDataExtractionController();
            var screenshotExtractionController = new ScreenshotExtractionController();

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
            var reportDirectoryDelegate = new FixedDirectoryDelegate("reports");
            var validationDirectoryDelegate = new FixedDirectoryDelegate("md5");
            var productFilesBaseDirectoryDelegate = new FixedDirectoryDelegate("productFiles");
            var screenshotsDirectoryDelegate = new FixedDirectoryDelegate("screenshots");

            var productFilesDirectoryDelegate = new UriDirectoryDelegate(productFilesBaseDirectoryDelegate);

            // filenames

            var indexFilenameDelegate = new FixedFilenameDelegate("index", jsonFilenameDelegate);

            var wishlistedFilenameDelegate = new FixedFilenameDelegate("wishlisted", jsonFilenameDelegate);
            var updatedFilenameDelegate = new FixedFilenameDelegate("updated", jsonFilenameDelegate);

            var uriFilenameDelegate = new UriFilenameDelegate();
            var reportFilenameDelegate = new ReportFilenameDelegate();
            var validationFilenameDelegate = new ValidationFilenameDelegate();

            // index filenames

            var productsIndexDataController = new IndexDataController(
                collectionController,
                productsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var accountProductsIndexDataController = new IndexDataController(
                collectionController,
                accountProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var gameDetailsIndexDataController = new IndexDataController(
                collectionController,
                gameDetailsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var gameProductDataIndexDataController = new IndexDataController(
                collectionController,
                gameProductDataDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var apiProductsIndexDataController = new IndexDataController(
                collectionController,
                apiProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var productScreenshotsIndexDataController = new IndexDataController(
                collectionController,
                productScreenshotsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var productDownloadsIndexDataController = new IndexDataController(
                collectionController,
                productDownloadsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var productRoutesIndexDataController = new IndexDataController(
                collectionController,
                productRoutesDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            // index data controllers that are data controllers

            var wishlistedDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                wishlistedFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

            var updatedDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                updatedFilenameDelegate,
                serializedStorageController,
                taskStatusAppController);

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
                taskStatusAppController);

            var accountProductsDataController = new DataController<AccountProduct>(
                accountProductsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                accountProductsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            var gameDetailsDataController = new DataController<GameDetails>(
                gameDetailsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                gameDetailsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            var gameProductDataController = new DataController<GameProductData>(
                gameProductDataIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                gameProductDataDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            var apiProductsDataController = new DataController<ApiProduct>(
                apiProductsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                apiProductsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                productScreenshotsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productScreenshotsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            var productDownloadsDataController = new DataController<ProductDownloads>(
                productDownloadsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productDownloadsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            var productRoutesDataController = new DataController<ProductRoutes>(
                productRoutesIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productRoutesDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                taskStatusAppController);

            #endregion

            #region Settings: Load, Validation

            var settingsFilenameDelegate = new FixedFilenameDelegate("settings", jsonFilenameDelegate);

            var settingsController = new SettingsController(
                settingsFilenameDelegate,
                serializedStorageController);

            var downloadsLanguagesValidationDelegate = new DownloadsLanguagesValidationDelegate(languageController);
            var downloadsOperatingSystemsValidationDelegate = new DownloadsOperatingSystemsValidationDelegate();

            var validateSettingsActivity = new ValidateSettingsActivity(
                settingsController,
                downloadsLanguagesValidationDelegate,
                downloadsOperatingSystemsValidationDelegate,
                taskStatusAppController);

            #endregion

            #region Activity Parameters 

            var activityParametersFilenameDelegate = new FixedFilenameDelegate("activityParameters", jsonFilenameDelegate);

            var activityParametersController = new ActivityParametersController(
                activityParametersFilenameDelegate,
                serializedStorageController);

            #endregion

            #region Task Activity Controllers

            #region Load

            var loadDataActivity = new LoadDataActivity(
                taskStatusAppController,
                // hash tracking should be first data controller to be loaded 
                // as all serialized storage operations go through it and might overwrite data on disk
                hashTrackingController,
                appTemplateController,
                reportTemplateController,
                settingsController,
                activityParametersController,
                productsDataController,
                accountProductsDataController,
                gameDetailsDataController,
                gameProductDataController,
                screenshotsDataController,
                apiProductsDataController,
                wishlistedDataController,
                updatedDataController,
                productDownloadsDataController,
                productRoutesDataController);

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

            var authorizeActivity = new AuthorizeActivity(
                settingsController,
                authorizationController,
                taskStatusAppController);

            #endregion

            #region Update.PageResults

            var productParameterGetUpdateUriDelegate = new ProductParameterUpdateUriDelegate();
            var productParameterGetQueryParametersDelegate = new ProductParameterGetQueryParametersDelegate();

            var productsPageResultsController = new PageResultsController<ProductsPageResult>(
                Parameters.Products,
                productParameterGetUpdateUriDelegate,
                productParameterGetQueryParametersDelegate,
                requestPageController,
                hashTrackingController,
                serializationController,
                taskStatusAppController);

            var productsExtractionController = new ProductsExtractionController();

            var productsUpdateActivity = new PageResultUpdateActivity<ProductsPageResult, Product>(
                    Parameters.Products,
                    productsPageResultsController,
                    productsExtractionController,
                    requestPageController,
                    productsDataController,
                    taskStatusAppController);

            var accountProductsPageResultsController = new PageResultsController<AccountProductsPageResult>(
                Parameters.AccountProducts,
                productParameterGetUpdateUriDelegate,
                productParameterGetQueryParametersDelegate,
                requestPageController,
                hashTrackingController,
                serializationController,
                taskStatusAppController);

            var accountProductsExtractionController = new AccountProductsExtractionController();

            var newUpdatedDataRefinementController = new NewUpdatedDataRefinementController(
                accountProductsDataController,
                collectionController,
                updatedDataController,
                taskStatusAppController);

            var accountProductsUpdateActivity = new PageResultUpdateActivity<AccountProductsPageResult, AccountProduct>(
                    Parameters.AccountProducts,
                    accountProductsPageResultsController,
                    accountProductsExtractionController,
                    requestPageController,
                    accountProductsDataController,
                    taskStatusAppController,
                    newUpdatedDataRefinementController);

            #endregion

            #region Update.Wishlisted

            var getProductsPageResultDelegate = new GetDeserializedGOGDataDelegate<ProductsPageResult>(networkController,
                gogDataExtractionController,
                serializationController);

            var wishlistedUpdateActivity = new WishlistedUpdateActivity(
                getProductsPageResultDelegate,
                wishlistedDataController,
                taskStatusAppController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var getGOGDataDelegate = new GetDeserializedGOGDataDelegate<GOGData>(networkController,
                gogDataExtractionController,
                serializationController);

            var getGameProductDataDeserializedDelegate = new GetGameProductDataDeserializedDelegate(
                getGOGDataDelegate);

            var productGetUpdateIdentityDelegate = new ProductGetUpdateIdentityDelegate();
            var productUrlGetUpdateIdentityDelegate = new ProductUrlGetUpdateIdentityDelegate();
            var accountProductGetUpdateIdentityDelegate = new AccountProductGetUpdateIdentityDelegate();

            var gameDetailsAccountProductConnectDelegate = new GameDetailsAccountProductConnectDelegate();

            // product update controllers

            var gameProductDataUpdateActivity = new ProductCoreUpdateActivity<GameProductData, Product>(
                Parameters.GameProductData,
                productParameterGetUpdateUriDelegate,
                gameProductDataController,
                productsDataController,
                updatedDataController,
                getGameProductDataDeserializedDelegate,
                productUrlGetUpdateIdentityDelegate,
                taskStatusAppController);

            var getApriProductDelegate = new GetDeserializedGOGModelDelegate<ApiProduct>(
                networkController,
                serializationController);

            var apiProductUpdateActivity = new ProductCoreUpdateActivity<ApiProduct, Product>(
                Parameters.ApiProducts,
                productParameterGetUpdateUriDelegate,
                apiProductsDataController,
                productsDataController,
                updatedDataController,
                getApriProductDelegate,
                productGetUpdateIdentityDelegate,
                taskStatusAppController);

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

            var gameDetailsUpdateActivity = new ProductCoreUpdateActivity<GameDetails, AccountProduct>(
                Parameters.GameDetails,
                productParameterGetUpdateUriDelegate,
                gameDetailsDataController,
                accountProductsDataController,
                updatedDataController,
                getGameDetailsDelegate,
                accountProductGetUpdateIdentityDelegate,
                taskStatusAppController,
                gameDetailsAccountProductConnectDelegate);

            #endregion

            #region Update.Screenshots

            var screenshotUpdateActivity = new ScreenshotUpdateActivity(
                productParameterGetUpdateUriDelegate,
                screenshotsDataController,
                productsDataController,
                networkController,
                screenshotExtractionController,
                taskStatusAppController);

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
                screenshotsDataController,
                screenshotUriController,
                screenshotsDirectoryDelegate,
                fileController,
                taskStatusAppController);

            var routingController = new RoutingController(productRoutesDataController);

            var gameDetailsManualUrlsEnumerateDelegate = new GameDetailsManualUrlEnumerateDelegate(
                settingsController,
                gameDetailsDataController);

            var gameDetailsDirectoryEnumerateDelegate = new GameDetailsDirectoryEnumerateDelegate(
                gameDetailsManualUrlsEnumerateDelegate,
                productFilesDirectoryDelegate);

            var gameDetailsFilesEnumerateDelegate = new GameDetailsFileEnumerateDelegate(
                gameDetailsManualUrlsEnumerateDelegate,
                routingController,
                productFilesDirectoryDelegate,
                uriFilenameDelegate);

            // product files are driven through gameDetails manual urls
            // so this sources enumerates all manual urls for all updated game details
            var manualUrlDownloadSourcesController = new ManualUrlDownloadSourcesController(
                updatedDataController,
                gameDetailsDataController,
                gameDetailsManualUrlsEnumerateDelegate);

            // schedule download controllers

            var updateProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.ProductsImages,
                productsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                taskStatusAppController);

            var updateAccountProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.AccountProductsImages,
                accountProductsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                taskStatusAppController);

            var updateScreenshotsDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.Screenshots,
                screenshotsDownloadSourcesController,
                screenshotsDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                taskStatusAppController);

            var updateProductFilesDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.ProductsFiles,
                manualUrlDownloadSourcesController,
                productFilesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                taskStatusAppController);

            // downloads processing

            var productsImagesDownloadActivity = new DownloadActivity(
                Parameters.ProductsImages,
                productDownloadsDataController,
                fileDownloadController,
                taskStatusAppController);

            var accountProductsImagesDownloadActivity = new DownloadActivity(
                Parameters.AccountProductsImages,
                productDownloadsDataController,
                fileDownloadController,
                taskStatusAppController);

            var screenshotsDownloadActivity = new DownloadActivity(
                Parameters.Screenshots,
                productDownloadsDataController,
                fileDownloadController,
                taskStatusAppController);

            var uriSansSessionExtractionController = new UriSansSessionExtractionController();

            var validationExpectedDelegate = new ValidationExpectedDelegate();

            var validationUriDelegate = new ValidationUriDelegate(
                validationFilenameDelegate,
                uriSansSessionExtractionController);

            var validationFileEnumerateDelegate = new ValidationFileEnumerateDelegate(
                validationDirectoryDelegate,
                validationFilenameDelegate);

            var validationDownloadFileFromSourceDelegate = new ValidationDownloadFileFromSourceDelegate(
                uriSansSessionExtractionController,
                validationExpectedDelegate,
                validationFileEnumerateDelegate,
                validationDirectoryDelegate,
                validationUriDelegate,
                fileController,
                fileDownloadController,
                taskStatusAppController);

            var manualUrlDownloadFromSourceDelegate = new ManualUrlDownloadFromSourceDelegate(
                networkController,
                uriSansSessionExtractionController,
                routingController,
                fileDownloadController,
                validationDownloadFileFromSourceDelegate,
                taskStatusAppController);

            var productFilesDownloadActivity = new DownloadActivity(
                Parameters.ProductsFiles,
                productDownloadsDataController,
                manualUrlDownloadFromSourceDelegate,
                taskStatusAppController);

            // validation controllers

            var validationController = new ValidationController(
                validationDirectoryDelegate,
                validationFilenameDelegate,
                fileController,
                streamController,
                md5HashController,
                taskStatusAppController);

            var validateActivity = new ValidateActivity(
                productFilesDirectoryDelegate,
                uriFilenameDelegate,
                validationController,
                gameDetailsDataController,
                gameDetailsManualUrlsEnumerateDelegate,
                validationExpectedDelegate,
                updatedDataController,
                routingController,
                taskStatusAppController);

            #region Cleanup

            var gameDetailsDirectoriesEnumerateDelegate = new GameDetailsDirectoriesEnumerateDelegate(
                gameDetailsDataController,
                gameDetailsDirectoryEnumerateDelegate,
                taskStatusAppController);

            var productFilesDirectoriesEnumerateDelegate = new ProductFilesDirectoriesEnumerateDelegate(
                productFilesBaseDirectoryDelegate,
                directoryController,
                taskStatusAppController);

            var directoryFilesEnumerateDelegate = new DirectoryFilesEnumerateDelegate(directoryController);

            var directoryCleanupActivity = new CleanupActivity(
                Parameters.Directories,
                gameDetailsDirectoriesEnumerateDelegate, // expected items (directories for gameDetails)
                productFilesDirectoriesEnumerateDelegate, // actual items (directories in productFiles)
                directoryFilesEnumerateDelegate, // detailed items (files in directory)
                validationFileEnumerateDelegate, // supplementary items (validation files)
                recycleBinController,
                directoryController,
                taskStatusAppController);

            var updatedGameDetailsManualUrlFilesEnumerateDelegate = new UpdatedGameDetailsManualUrlFilesEnumerateDelegate(
                updatedDataController,
                gameDetailsDataController,
                gameDetailsFilesEnumerateDelegate,
                taskStatusAppController);

            var updatedProductFilesEnumerateDelegate = new UpdatedProductFilesEnumerateDelegate(
                updatedDataController,
                gameDetailsDataController,
                gameDetailsDirectoryEnumerateDelegate,
                directoryController,
                taskStatusAppController);

            var passthroughEnumerateDelegate = new PassthroughEnumerateDelegate();

            var fileCleanupActivity = new CleanupActivity(
                Parameters.Files,
                updatedGameDetailsManualUrlFilesEnumerateDelegate, // expected items (files for updated gameDetails)
                updatedProductFilesEnumerateDelegate, // actual items (updated product files)
                passthroughEnumerateDelegate, // detailed items (passthrough)
                validationFileEnumerateDelegate, // supplementary items (validation files)
                recycleBinController,
                directoryController,
                taskStatusAppController);

            var cleanupUpdatedActivity = new CleanupUpdatedActivity(
                updatedDataController,
                taskStatusAppController);

            #endregion

            #region Report Task Status 

            var reportFilePresentationController = new FilePresentationController(
                reportDirectoryDelegate,
                reportFilenameDelegate,
                streamController);

            var taskStatusReportViewController = new TaskStatusViewController(
                applicationTaskStatus,
                reportTemplateController,
                taskStatusReportViewModelDelegate,
                taskStatusTreeToEnumerableController,
                reportFilePresentationController);

            var reportActivity = new ReportActivity(
                taskStatusReportViewController,
                taskStatusAppController);

            #endregion

            #endregion

            #region Task Activities Parameters

            var activityParametersNameDelegate = new ActivityParametersNameDelegate();

            var activityParametersTaskActivities = new Dictionary<string, ITaskActivityController>()
            {
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.Products),
                    productsUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.AccountProducts),
                    accountProductsUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.Wishlist),
                    wishlistedUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.GameProductData),
                    gameProductDataUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.ApiProducts),
                    apiProductUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.GameDetails),
                    gameDetailsUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.Screenshots),
                    screenshotUpdateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.ProductsImages),
                    updateProductsImagesDownloadsActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.AccountProductsImages),
                    updateAccountProductsImagesDownloadsActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.Screenshots),
                    updateScreenshotsDownloadsActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.ProductsFiles),
                    updateProductFilesDownloadsActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.ProductsImages),
                    productsImagesDownloadActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.AccountProductsImages),
                    accountProductsImagesDownloadActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.Screenshots),
                    screenshotsDownloadActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.ProductsFiles),
                    productFilesDownloadActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.Validate,
                    Parameters.ProductsFiles),
                    validateActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.Cleanup,
                    Parameters.Directories),
                    directoryCleanupActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.Cleanup,
                    Parameters.Files),
                    fileCleanupActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.Cleanup,
                    Parameters.Updated),
                    cleanupUpdatedActivity },
                { activityParametersNameDelegate.GetName(
                    Activities.Report,
                    Parameters.TaskStatus),
                    reportActivity }
            };

            var processActivityParametersActivity = new ProcessActivityParametersActivity(
                activityParametersController,
                activityParametersNameDelegate,
                activityParametersTaskActivities,
                taskStatusAppController);

            #endregion

            #region Initialization Task Activities (always performed)

            var activities = new List<ITaskActivityController>
            {
                // load initial data
                loadDataActivity,
                // validate settings
                validateSettingsActivity,
                // authorize
                authorizeActivity,
                //  activity parameters
                processActivityParametersActivity
            };

            #endregion

            foreach (var activity in activities)
            {
                try
                {
                    activity.ProcessTaskAsync(applicationTaskStatus).Wait();
                }
                catch (AggregateException ex)
                {
                    List<string> errorMessages = new List<string>();

                    foreach (var innerException in ex.InnerExceptions)
                        errorMessages.Add(innerException.Message);

                    taskStatusAppController.Fail(applicationTaskStatus, string.Join(", ", errorMessages));
                }
            }

            consolePresentationController.Present(
                new string[]
                    {"All GoodOfflineGames tasks are complete.\n\nPress ENTER to close the window..."});

            consoleController.ReadLine();
        }
    }
}
