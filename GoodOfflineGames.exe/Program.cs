#region Using

using System;
using System.IO;
using System.Net;
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
using Controllers.FlightPlan;
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
using Controllers.PropertyValidation;
using Controllers.Validation;
using Controllers.ValidationResult;
using Controllers.Conversion;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Indexing;
using Controllers.Presentation;
using Controllers.RecycleBin;
using Controllers.Routing;
using Controllers.Status;
using Controllers.StatusProgress;
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

using Interfaces.Activity;
using Interfaces.Extraction;

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
using GOG.Controllers.UpdateScreenshots;

using GOG.Activities.Load;
using GOG.Activities.ValidateSettings;
using GOG.Activities.Authorize;
using GOG.Activities.Flight;
using GOG.Activities.UpdateData;
using GOG.Activities.UpdateDownloads;
using GOG.Activities.Download;
using GOG.Activities.Repair;
using GOG.Activities.Cleanup;
using GOG.Activities.Validate;
using GOG.Activities.Report;

using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;
using Models.ValidationResult;
using Models.Status;
using Models.FlightPlan;
using Models.QueryParameters;
using Models.Directories;

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

            var precomputedHashController = new PrecomputedHashController(
                uriHashesFilenameDelegate,
                serializationController,
                storageController);

            var bytesToStringConversionController = new BytesToStringConvertionController();
            var bytesMd5Controller = new BytesMd5Controller(bytesToStringConversionController);
            var stringToBytesConversionController = new StringToBytesConversionController();
            var stringMd5Controller = new StringMd5Controller(stringToBytesConversionController, bytesMd5Controller);

            var serializedStorageController = new SerializedStorageController(
                precomputedHashController,
                storageController,
                stringMd5Controller,
                serializationController);

            var consoleController = new ConsoleController();
            var lineBreakingController = new LineBreakingController();

            var consolePresentationController = new ConsolePresentationController(
                lineBreakingController,
                consoleController);

            var bytesFormattingController = new BytesFormattingController();
            var secondsFormattingController = new SecondsFormattingController();

            var collectionController = new CollectionController();

            var statusTreeToEnumerableController = new StatusTreeToEnumerableController();

            var applicationStatus = new Status() { Title = "Welcome to GoodOfflineGames" };

            var templatesDirectoryDelegate = new RelativeDirectoryDelegate("templates");
            var appTemplateFilenameDelegate = new FixedFilenameDelegate("app", jsonFilenameDelegate);
            var reportTemplateFilenameDelegate = new FixedFilenameDelegate("report", jsonFilenameDelegate);

            var appTemplateController = new TemplateController(
                "status",
                templatesDirectoryDelegate,
                appTemplateFilenameDelegate,
                serializedStorageController,
                collectionController);

            var reportTemplateController = new TemplateController(
                "status",
                templatesDirectoryDelegate,
                reportTemplateFilenameDelegate,
                serializedStorageController,
                collectionController);

            var statusProgressController = new StatusProgressController();

            var statusAppViewModelDelegate = new StatusAppViewModelDelegate(
                statusProgressController,
                bytesFormattingController,
                secondsFormattingController);

            var statusReportViewModelDelegate = new StatusReportViewModelDelegate(
                bytesFormattingController,
                secondsFormattingController);

            var statusAppViewController = new StatusViewController(
                applicationStatus,
                appTemplateController,
                statusAppViewModelDelegate,
                statusTreeToEnumerableController,
                consolePresentationController);

            var statusController = new StatusController(statusAppViewController);

            var throttleController = new ThrottleController(
                statusController,
                secondsFormattingController);

            var requestRateController = new RequestRateController(
                throttleController,
                collectionController,
                statusController,
                new string[] {
                    Models.Uris.Uris.Paths.Account.GameDetails, // gameDetails requests
                    Models.Uris.Uris.Paths.ProductFiles.ManualUrlDownlink, // manualUrls from gameDetails requests
                    Models.Uris.Uris.Paths.ProductFiles.ManualUrlCDNSecure, // resolved manualUrls and validation files requests
                    Models.Uris.Uris.Roots.Api // API entries
                });

            var uriController = new UriController();

            var cookieContainer = new CookieContainer();
            var cookiesFilenameDelegate = new FixedFilenameDelegate("cookies", jsonFilenameDelegate);

            var cookieContainerSerializationController = new CookieContainerSerializationController(
                ref cookieContainer,
                cookiesFilenameDelegate,
                storageController);

            var networkController = new NetworkController(
                ref cookieContainer,
                cookieContainerSerializationController,
                uriController,
                requestRateController);

            var fileDownloadController = new FileDownloadController(
                networkController,
                streamController,
                fileController,
                statusController);

            var requestPageController = new RequestPageController(
                networkController);

            var languageController = new LanguageController();

            var loginIdExtractionController = new LoginIdExtractionController();
            var loginUsernameExtractionController = new LoginUsernameExtractionController();
            var loginUnderscoreTokenExtractionController = new LoginTokenExtractionController();
            var secondStepAuthenticationUnderscoreTokenExtractionController = new SecondStepAuthenticationTokenExtractionController();

            var gogDataExtractionController = new GOGDataExtractionController();
            var screenshotExtractionController = new ScreenshotExtractionController();

            var imageUriController = new ImageUriController();
            var screenshotUriController = new ScreenshotUriController();

            #endregion

            #region Data Controllers

            // Data controllers for products, game details, game product data, etc.

            var productCoreIndexingController = new ProductCoreIndexingController();

            // directories

            var settingsFilenameDelegate = new FixedFilenameDelegate("settings", jsonFilenameDelegate);

            var settingsController = new SettingsController(
                settingsFilenameDelegate,
                serializedStorageController);

            var downloadsLanguagesValidationDelegate = new DownloadsLanguagesValidationDelegate(languageController);
            var downloadsOperatingSystemsValidationDelegate = new DownloadsOperatingSystemsValidationDelegate();
            var directoriesValidationDelegate = new DirectoriesValidationDelegate();

            var validateSettingsActivity = new ValidateSettingsActivity(
                settingsController,
                downloadsLanguagesValidationDelegate,
                downloadsOperatingSystemsValidationDelegate,
                directoriesValidationDelegate,
                statusController);

            var dataDirectoryDelegate = new SettingsDirectoryDelegate(Directories.Data, settingsController);

            var accountProductsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.AccountProducts, dataDirectoryDelegate);
            var apiProductsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.ApiProducts, dataDirectoryDelegate);
            var gameDetailsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.GameDetails, dataDirectoryDelegate);
            var gameProductDataDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.GameProductData, dataDirectoryDelegate);
            var productsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.Products, dataDirectoryDelegate);
            var productDownloadsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.ProductDownloads, dataDirectoryDelegate);
            var productRoutesDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.ProductRoutes, dataDirectoryDelegate);
            var productScreenshotsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.ProductScreenshots, dataDirectoryDelegate);
            var validationResultsDirectoryDelegate = new RelativeDirectoryDelegate(DataDirectories.ValidationResults, dataDirectoryDelegate);

            var recycleBinDirectoryDelegate = new SettingsDirectoryDelegate(Directories.RecycleBin, settingsController);
            var imagesDirectoryDelegate = new SettingsDirectoryDelegate(Directories.Images, settingsController);
            var reportDirectoryDelegate = new SettingsDirectoryDelegate(Directories.Reports, settingsController);
            var validationDirectoryDelegate = new SettingsDirectoryDelegate(Directories.Md5, settingsController);
            var productFilesBaseDirectoryDelegate = new SettingsDirectoryDelegate(Directories.ProductFiles, settingsController);
            var screenshotsDirectoryDelegate = new SettingsDirectoryDelegate(Directories.Screenshots, settingsController);

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
                statusController);

            var accountProductsIndexDataController = new IndexDataController(
                collectionController,
                accountProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var gameDetailsIndexDataController = new IndexDataController(
                collectionController,
                gameDetailsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var gameProductDataIndexDataController = new IndexDataController(
                collectionController,
                gameProductDataDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var apiProductsIndexDataController = new IndexDataController(
                collectionController,
                apiProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var productScreenshotsIndexDataController = new IndexDataController(
                collectionController,
                productScreenshotsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var productDownloadsIndexDataController = new IndexDataController(
                collectionController,
                productDownloadsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var productRoutesIndexDataController = new IndexDataController(
                collectionController,
                productRoutesDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            var validationResultsIndexController = new IndexDataController(
                collectionController,
                validationResultsDirectoryDelegate,
                indexFilenameDelegate,
                serializedStorageController,
                statusController);

            // index data controllers that are data controllers

            var wishlistedDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                wishlistedFilenameDelegate,
                serializedStorageController,
                statusController);

            var updatedDataController = new IndexDataController(
                collectionController,
                dataDirectoryDelegate,
                updatedFilenameDelegate,
                serializedStorageController,
                statusController);

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
                statusController);

            var accountProductsDataController = new DataController<AccountProduct>(
                accountProductsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                accountProductsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var gameDetailsDataController = new DataController<GameDetails>(
                gameDetailsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                gameDetailsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var gameProductDataController = new DataController<GameProductData>(
                gameProductDataIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                gameProductDataDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var apiProductsDataController = new DataController<ApiProduct>(
                apiProductsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                apiProductsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                productScreenshotsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productScreenshotsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var productDownloadsDataController = new DataController<ProductDownloads>(
                productDownloadsIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productDownloadsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var productRoutesDataController = new DataController<ProductRoutes>(
                productRoutesIndexDataController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                productRoutesDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            var validationResultsDataController = new DataController<ValidationResult>(
                validationResultsIndexController,
                serializedStorageController,
                productCoreIndexingController,
                collectionController,
                validationResultsDirectoryDelegate,
                jsonFilenameDelegate,
                recycleBinController,
                statusController);

            #endregion

            #region Activity Parameters 

            var flightPlanFilenameDelegate = new FixedFilenameDelegate("flightPlan", jsonFilenameDelegate);

            var flightPlanController = new FlightPlanController(
                flightPlanFilenameDelegate,
                serializedStorageController);

            #endregion

            #region Activities

            #region Load

            var loadDataActivity = new LoadDataActivity(
                statusController,
                settingsController,
                precomputedHashController,
                appTemplateController,
                reportTemplateController,
                flightPlanController,
                productsDataController,
                accountProductsDataController,
                gameDetailsDataController,
                gameProductDataController,
                screenshotsDataController,
                apiProductsDataController,
                wishlistedDataController,
                updatedDataController,
                productDownloadsDataController,
                productRoutesDataController,
                validationResultsDataController);

            #endregion

            #region Authorization

            var usernamePasswordValidationDelegate = new UsernamePasswordValidationDelegate(consoleController);
            var securityCodeValidationDelegate = new SecurityCodeValidationDelegate(consoleController);

            var authorizationExtractionControllers = new Dictionary<string, IStringExtractionController>()
            {
                { QueryParameters.LoginId,
                    loginIdExtractionController },
                { QueryParameters.LoginUsername,
                    loginUsernameExtractionController },
                { QueryParameters.LoginUnderscoreToken,
                    loginUnderscoreTokenExtractionController },
                { QueryParameters.SecondStepAuthenticationUnderscoreToken,
                    secondStepAuthenticationUnderscoreTokenExtractionController }
            };

            var authorizationController = new AuthorizationController(
                usernamePasswordValidationDelegate,
                securityCodeValidationDelegate,
                uriController,
                networkController,
                serializationController,
                authorizationExtractionControllers,
                statusController);

            var authorizeActivity = new AuthorizeActivity(
                settingsController,
                authorizationController,
                statusController);

            #endregion

            #region Update.PageResults

            var productParameterGetUpdateUriDelegate = new ProductParameterUpdateUriDelegate();
            var productParameterGetQueryParametersDelegate = new ProductParameterGetQueryParametersDelegate();

            var productsPageResultsController = new PageResultsController<ProductsPageResult>(
                Parameters.Products,
                productParameterGetUpdateUriDelegate,
                productParameterGetQueryParametersDelegate,
                requestPageController,
                stringMd5Controller,
                precomputedHashController,
                serializationController,
                statusController);

            var productsExtractionController = new ProductsExtractionController();

            var productsUpdateActivity = new PageResultUpdateActivity<ProductsPageResult, Product>(
                    Parameters.Products,
                    productsPageResultsController,
                    productsExtractionController,
                    requestPageController,
                    productsDataController,
                    statusController);

            var accountProductsPageResultsController = new PageResultsController<AccountProductsPageResult>(
                Parameters.AccountProducts,
                productParameterGetUpdateUriDelegate,
                productParameterGetQueryParametersDelegate,
                requestPageController,
                stringMd5Controller,
                precomputedHashController,
                serializationController,
                statusController);

            var accountProductsExtractionController = new AccountProductsExtractionController();

            var newUpdatedDataRefinementController = new NewUpdatedDataRefinementController(
                accountProductsDataController,
                collectionController,
                updatedDataController,
                statusController);

            var accountProductsUpdateActivity = new PageResultUpdateActivity<AccountProductsPageResult, AccountProduct>(
                    Parameters.AccountProducts,
                    accountProductsPageResultsController,
                    accountProductsExtractionController,
                    requestPageController,
                    accountProductsDataController,
                    statusController,
                    newUpdatedDataRefinementController);

            #endregion

            #region Update.Wishlisted

            var getProductsPageResultDelegate = new GetDeserializedGOGDataDelegate<ProductsPageResult>(networkController,
                gogDataExtractionController,
                serializationController);

            var wishlistedUpdateActivity = new WishlistedUpdateActivity(
                getProductsPageResultDelegate,
                wishlistedDataController,
                statusController);

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
                statusController);

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
                statusController);

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
                statusController,
                gameDetailsAccountProductConnectDelegate);

            #endregion

            #region Update.Screenshots

            var updateProductScreenshotsDelegate = new UpdateProductScreenshotsDelegate(
                productParameterGetUpdateUriDelegate,
                screenshotsDataController,
                networkController,
                screenshotExtractionController,
                statusController);

            var screenshotUpdateActivity = new ScreenshotUpdateActivity(
                productsDataController,
                productScreenshotsIndexDataController,
                updateProductScreenshotsDelegate,
                statusController);

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
                statusController);

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
                statusController);

            var updateAccountProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.AccountProductsImages,
                accountProductsImagesDownloadSourcesController,
                imagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateScreenshotsDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.Screenshots,
                screenshotsDownloadSourcesController,
                screenshotsDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateProductFilesDownloadsActivity = new UpdateDownloadsActivity(
                Parameters.ProductsFiles,
                manualUrlDownloadSourcesController,
                productFilesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            // downloads processing

            var productsImagesDownloadActivity = new DownloadActivity(
                Parameters.ProductsImages,
                productDownloadsDataController,
                fileDownloadController,
                statusController);

            var accountProductsImagesDownloadActivity = new DownloadActivity(
                Parameters.AccountProductsImages,
                productDownloadsDataController,
                fileDownloadController,
                statusController);

            var screenshotsDownloadActivity = new DownloadActivity(
                Parameters.Screenshots,
                productDownloadsDataController,
                fileDownloadController,
                statusController);

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
                statusController);

            var manualUrlDownloadFromSourceDelegate = new ManualUrlDownloadFromSourceDelegate(
                networkController,
                uriSansSessionExtractionController,
                routingController,
                fileDownloadController,
                validationDownloadFileFromSourceDelegate,
                statusController);

            var productFilesDownloadActivity = new DownloadActivity(
                Parameters.ProductsFiles,
                productDownloadsDataController,
                manualUrlDownloadFromSourceDelegate,
                statusController);

            // validation controllers

            var validationController = new ValidationController(
                validationExpectedDelegate,
                fileController,
                streamController,
                bytesMd5Controller,
                statusController);

            var validateActivity = new ValidateActivity(
                productFilesDirectoryDelegate,
                uriFilenameDelegate,
                validationFileEnumerateDelegate,
                validationController,
                validationResultsDataController,
                gameDetailsDataController,
                gameDetailsManualUrlsEnumerateDelegate,
                updatedDataController,
                routingController,
                statusController);

            #region Repair

            var validationResultController = new ValidationResultController();

            var repairActivity = new RepairActivity(
                validationResultsDataController,
                validationResultController,
                statusController);

            #endregion

            #region Cleanup

            var gameDetailsDirectoriesEnumerateDelegate = new GameDetailsDirectoriesEnumerateDelegate(
                gameDetailsDataController,
                gameDetailsDirectoryEnumerateDelegate,
                statusController);

            var productFilesDirectoriesEnumerateDelegate = new ProductFilesDirectoriesEnumerateDelegate(
                productFilesBaseDirectoryDelegate,
                directoryController,
                statusController);

            var directoryFilesEnumerateDelegate = new DirectoryFilesEnumerateDelegate(directoryController);

            var directoryCleanupActivity = new CleanupActivity(
                Parameters.Directories,
                gameDetailsDirectoriesEnumerateDelegate, // expected items (directories for gameDetails)
                productFilesDirectoriesEnumerateDelegate, // actual items (directories in productFiles)
                directoryFilesEnumerateDelegate, // detailed items (files in directory)
                validationFileEnumerateDelegate, // supplementary items (validation files)
                recycleBinController,
                directoryController,
                statusController);

            var updatedGameDetailsManualUrlFilesEnumerateDelegate = new UpdatedGameDetailsManualUrlFilesEnumerateDelegate(
                updatedDataController,
                gameDetailsDataController,
                gameDetailsFilesEnumerateDelegate,
                statusController);

            var updatedProductFilesEnumerateDelegate = new UpdatedProductFilesEnumerateDelegate(
                updatedDataController,
                gameDetailsDataController,
                gameDetailsDirectoryEnumerateDelegate,
                directoryController,
                statusController);

            var passthroughEnumerateDelegate = new PassthroughEnumerateDelegate();

            var fileCleanupActivity = new CleanupActivity(
                Parameters.Files,
                updatedGameDetailsManualUrlFilesEnumerateDelegate, // expected items (files for updated gameDetails)
                updatedProductFilesEnumerateDelegate, // actual items (updated product files)
                passthroughEnumerateDelegate, // detailed items (passthrough)
                validationFileEnumerateDelegate, // supplementary items (validation files)
                recycleBinController,
                directoryController,
                statusController);

            var cleanupUpdatedActivity = new CleanupUpdatedActivity(
                updatedDataController,
                statusController);

            #endregion

            #region Report Task Status 

            var reportFilePresentationController = new FilePresentationController(
                reportDirectoryDelegate,
                reportFilenameDelegate,
                streamController);

            var statusReportViewController = new StatusViewController(
                applicationStatus,
                reportTemplateController,
                statusReportViewModelDelegate,
                statusTreeToEnumerableController,
                reportFilePresentationController);

            var reportActivity = new ReportActivity(
                statusReportViewController,
                statusController);

            #endregion

            #endregion

            #region Task Activities Parameters

            var nameDelegate = new NameDelegate();

            var flightActivities = new Dictionary<string, IActivity>()
            {
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.Products),
                    productsUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.AccountProducts),
                    accountProductsUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.Wishlist),
                    wishlistedUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.GameProductData),
                    gameProductDataUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.ApiProducts),
                    apiProductUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.GameDetails),
                    gameDetailsUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateData,
                    Parameters.Screenshots),
                    screenshotUpdateActivity },
                { nameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.ProductsImages),
                    updateProductsImagesDownloadsActivity },
                { nameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.AccountProductsImages),
                    updateAccountProductsImagesDownloadsActivity },
                { nameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.Screenshots),
                    updateScreenshotsDownloadsActivity },
                { nameDelegate.GetName(
                    Activities.UpdateDownloads,
                    Parameters.ProductsFiles),
                    updateProductFilesDownloadsActivity },
                { nameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.ProductsImages),
                    productsImagesDownloadActivity },
                { nameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.AccountProductsImages),
                    accountProductsImagesDownloadActivity },
                { nameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.Screenshots),
                    screenshotsDownloadActivity },
                { nameDelegate.GetName(
                    Activities.ProcessDownloads,
                    Parameters.ProductsFiles),
                    productFilesDownloadActivity },
                { nameDelegate.GetName(
                    Activities.Validate,
                    Parameters.ProductsFiles),
                    validateActivity },
                { nameDelegate.GetName(
                    Activities.Repair,
                    Parameters.ProductsFiles),
                    repairActivity },
                { nameDelegate.GetName(
                    Activities.Cleanup,
                    Parameters.Directories),
                    directoryCleanupActivity },
                { nameDelegate.GetName(
                    Activities.Cleanup,
                    Parameters.Files),
                    fileCleanupActivity },
                { nameDelegate.GetName(
                    Activities.Cleanup,
                    Parameters.Updated),
                    cleanupUpdatedActivity },
                { nameDelegate.GetName(
                    Activities.Report,
                    Parameters.Status),
                    reportActivity }
            };

            var flightActivity = new FlightActivity(
                flightPlanController,
                nameDelegate,
                flightActivities,
                statusController);

            #endregion

            #region Initialization Task Activities (always performed)

            var activities = new List<IActivity>
            {
                // load initial data
                loadDataActivity,
                // validate settings
                validateSettingsActivity,
                // authorize
                authorizeActivity,
                //  flight plan
                flightActivity
            };

            #endregion

            foreach (var activity in activities)
            {
                try
                {
                    activity.ProcessActivityAsync(applicationStatus).Wait();
                }
                catch (AggregateException ex)
                {
                    var exceptionTreeToEnumerableController = new ExceptionTreeToEnumerableController();

                    List<string> errorMessages = new List<string>();
                    foreach (var innerException in exceptionTreeToEnumerableController.ToEnumerable(ex))
                        errorMessages.Add(innerException.Message);

                    var combinedErrorMessages = string.Join(", ", errorMessages);

                    statusController.Fail(applicationStatus, combinedErrorMessages);

                    var failureDumpUri = "failureDump.json";
                    serializedStorageController.SerializePushAsync(failureDumpUri, applicationStatus).Wait();

                    consolePresentationController.Present(
                        new string[]
                            {"GoodOfflineGames.exe has encountered fatal error(s):\n" + 
                            combinedErrorMessages +
                            $"\nPlease refer to {failureDumpUri} for further details."+
                            "\n\nPress ENTER to close the window..."});

                    consoleController.ReadLine();

                    return;
                }
            }

            consolePresentationController.Present(
                new string[]
                    {"All GoodOfflineGames tasks are complete.\n\nPress ENTER to close the window..."});

            consoleController.ReadLine();
        }
    }
}
