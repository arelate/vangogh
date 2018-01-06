#region Using

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Delegates.BreakLines;
using Delegates.GetIndex;
using Delegates.Convert;
using Delegates.Throttle;
using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.GetUri;

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
using Controllers.RequestPage;
using Controllers.RequestRate;
using Controllers.ImageUri;
using Controllers.Formatting;
using Controllers.Cookies;
using Controllers.PropertyValidation;
using Controllers.Validation;
using Controllers.ValidationResult;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Presentation;
using Controllers.RecycleBin;
using Controllers.Routing;
using Controllers.Status;
using Controllers.StatusRemainingTime;
using Controllers.Hash;
using Controllers.Containment;
using Controllers.Sanitization;
using Controllers.Expectation;
using Controllers.QueryParameters;
using Controllers.Template;
using Controllers.ViewModel;
using Controllers.Tree;
using Controllers.ViewUpdates;
using Controllers.Enumeration;
using Controllers.ActivityContext;
using Controllers.UserRequested;
using Controllers.InputOutput;

using Interfaces.Activity;
using Interfaces.Extraction;
using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using GOG.Models;

using GOG.Delegates.Extract;
using GOG.Delegates.ExtractPageResults;
using GOG.Delegates.GetPageResults;
using GOG.Delegates.FillGaps;
using GOG.Delegates.GetDownloadSources;
using GOG.Delegates.GetUpdateIdentity;
using GOG.Delegates.DownloadFileFromSource;
using GOG.Delegates.GetImageUri;
using GOG.Delegates.UpdateScreenshots;
using GOG.Delegates.GetDeserialized;
using GOG.Delegates.Enumerate;
using GOG.Delegates.EnumerateAll;
using GOG.Delegates.GetUpdateUri;

using GOG.Controllers.NewUpdatedSelection;
using GOG.Controllers.Authorization;

using GOG.Activities.Help;
using GOG.Activities.ValidateSettings;
using GOG.Activities.Authorize;
using GOG.Activities.UpdateData;
using GOG.Activities.UpdateDownloads;
using GOG.Activities.DownloadProductFiles;
using GOG.Activities.Repair;
using GOG.Activities.Cleanup;
using GOG.Activities.Validate;
using GOG.Activities.Report;
using GOG.Activities.List;

using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;
using Models.ValidationResult;
using Models.Status;
using Models.QueryParameters;
using Models.Directories;
using Models.ActivityContext;

#endregion

namespace Ghost.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region Foundation Controllers

            var statusController = new StatusController();

            var streamController = new StreamController();
            var fileController = new FileController();
            var directoryController = new DirectoryController();

            var storageController = new StorageController(
                streamController,
                fileController);
            var transactionalStorageController = new TransactionalStorageController(
                storageController,
                fileController);
            var serializationController = new JSONStringController();

            var getJsonFilenameDelegate = new GetJsonFilenameDelegate();
            var getUriHashesFilenameDelegate = new GetFixedFilenameDelegate("hashes", getJsonFilenameDelegate);

            var precomputedHashController = new PrecomputedHashController(
                getUriHashesFilenameDelegate,
                serializationController,
                transactionalStorageController);

            var convertBytesToStringDelegate = new ConvertBytesToStringDelegate();
            var bytesMd5Controller = new BytesMd5Controller(convertBytesToStringDelegate);
            var convertStringToBytesDelegate = new ConvertStringToBytesDelegate();
            var stringMd5Controller = new StringMd5Controller(convertStringToBytesDelegate, bytesMd5Controller);

            var consoleController = new ConsoleController();
            var breakLinesDelegate = new BreakLinesDelegate();

            var consoleInputOutputController = new ConsoleInputOutputController(
                breakLinesDelegate,
                consoleController);

            var bytesFormattingController = new BytesFormattingController();
            var secondsFormattingController = new SecondsFormattingController();

            var serializedStorageController = new SerializedStorageController(
                precomputedHashController,
                storageController,
                stringMd5Controller,
                serializationController,
                statusController);

            var serializedTransactionalStorageController = new SerializedStorageController(
                precomputedHashController,
                transactionalStorageController,
                stringMd5Controller,
                serializationController,
                statusController);

            var collectionController = new CollectionController();

            var statusTreeToEnumerableController = new StatusTreeToEnumerableController();

            var applicationStatus = new Status() { Title = "This ghost is a kind one." };

            var templatesDirectoryDelegate = new GetRelativeDirectoryDelegate("templates");
            var appTemplateFilenameDelegate = new GetFixedFilenameDelegate("app", getJsonFilenameDelegate);
            var reportTemplateFilenameDelegate = new GetFixedFilenameDelegate("report", getJsonFilenameDelegate);

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

            var getRemainingTimeAtUnitsPerSecondDelegate = new GetRemainingTimeAtUnitsPerSecondDelegate();

            var statusAppViewModelDelegate = new StatusAppViewModelDelegate(
                getRemainingTimeAtUnitsPerSecondDelegate,
                bytesFormattingController,
                secondsFormattingController);

            var statusReportViewModelDelegate = new StatusReportViewModelDelegate(
                bytesFormattingController,
                secondsFormattingController);

            var getStatusViewUpdateDelegate = new GetStatusViewUpdateDelegate(
                applicationStatus,
                appTemplateController,
                statusAppViewModelDelegate,
                statusTreeToEnumerableController);

            var consoleNotifyStatusViewUpdateController = new NotifyStatusViewUpdateController(
                getStatusViewUpdateDelegate,
                consoleInputOutputController);

            // add notification handler to drive console view updates
            statusController.NotifyStatusChangedAsync += consoleNotifyStatusViewUpdateController.NotifyViewUpdateOutputOnRefreshAsync;

            var throttleAsyncDelegate = new ThrottleAsyncDelegate(
                statusController,
                secondsFormattingController);

            var requestRateController = new RequestRateController(
                throttleAsyncDelegate,
                collectionController,
                statusController,
                new string[] {
                    Models.Uris.Uris.Paths.Account.GameDetails, // gameDetails requests
                    Models.Uris.Uris.Paths.ProductFiles.ManualUrlDownlink, // manualUrls from gameDetails requests
                    Models.Uris.Uris.Paths.ProductFiles.ManualUrlCDNSecure, // resolved manualUrls and validation files requests
                    Models.Uris.Uris.Roots.Api // API entries
                });

            var uriController = new UriController();

            var cookiesFilenameDelegate = new GetFixedFilenameDelegate("cookies", getJsonFilenameDelegate);
            var cookieSerializationController = new CookieSerializationController();

            var cookiesController = new CookiesController(
                cookieSerializationController,
                serializedStorageController,
                cookiesFilenameDelegate,
                statusController);

            var networkController = new NetworkController(
                cookiesController,
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

            var expandImageUriDelegate = new ExpandImageUriDelegate();
            var expandScreenshotUriDelegate = new ExpandScreenshotUriDelegate();

            #endregion

            #region Data Controllers

            // Data controllers for products, game details, game product data, etc.

            var getProductIndexDelegate = new GetProductCoreIndexDelegate<Product>();
            var getAccountProductIndexDelegate = new GetProductCoreIndexDelegate<AccountProduct>();
            var getGameDetailsIndexDelegate = new GetProductCoreIndexDelegate<GameDetails>();
            var getGameProductDataIndexDelegate = new GetProductCoreIndexDelegate<GameProductData>();
            var getApiProductIndexDelegate = new GetProductCoreIndexDelegate<ApiProduct>();
            var getProductScreenshotsIndexDelegate = new GetProductCoreIndexDelegate<ProductScreenshots>();
            var getProductDownloadsIndexDelegate = new GetProductCoreIndexDelegate<ProductDownloads>();
            var getProductRoutesIndexDelegate = new GetProductCoreIndexDelegate<ProductRoutes>();
            var getValidationResultIndexDelegate = new GetProductCoreIndexDelegate<ValidationResult>();

            // directories

            var settingsFilenameDelegate = new GetFixedFilenameDelegate("settings", getJsonFilenameDelegate);

            var settingsController = new SettingsController(
                settingsFilenameDelegate,
                serializedStorageController,
                statusController);

            var downloadsLanguagesValidationDelegate = new DownloadsLanguagesValidationDelegate(languageController);
            var downloadsOperatingSystemsValidationDelegate = new DownloadsOperatingSystemsValidationDelegate();
            var directoriesValidationDelegate = new DirectoriesValidationDelegate();

            var validateSettingsActivity = new ValidateSettingsActivity(
                settingsController,
                downloadsLanguagesValidationDelegate,
                downloadsOperatingSystemsValidationDelegate,
                directoriesValidationDelegate,
                statusController);

            var dataDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.Data, settingsController);

            var accountProductsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.AccountProducts, dataDirectoryDelegate);
            var apiProductsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.ApiProducts, dataDirectoryDelegate);
            var gameDetailsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.GameDetails, dataDirectoryDelegate);
            var gameProductDataDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.GameProductData, dataDirectoryDelegate);
            var productsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.Products, dataDirectoryDelegate);
            var productDownloadsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.ProductDownloads, dataDirectoryDelegate);
            var productRoutesDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.ProductRoutes, dataDirectoryDelegate);
            var productScreenshotsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.ProductScreenshots, dataDirectoryDelegate);
            var validationResultsDirectoryDelegate = new GetRelativeDirectoryDelegate(DataDirectories.ValidationResults, dataDirectoryDelegate);

            var recycleBinDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.RecycleBin, settingsController);
            var imagesDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.Images, settingsController);
            var reportDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.Reports, settingsController);
            var validationDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.Md5, settingsController);
            var productFilesBaseDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.ProductFiles, settingsController);
            var screenshotsDirectoryDelegate = new GetSettingsDirectoryDelegate(Directories.Screenshots, settingsController);

            var productFilesDirectoryDelegate = new GetUriDirectoryDelegate(productFilesBaseDirectoryDelegate);

            // filenames

            var indexFilenameDelegate = new GetFixedFilenameDelegate("index", getJsonFilenameDelegate);

            var wishlistedFilenameDelegate = new GetFixedFilenameDelegate("wishlisted", getJsonFilenameDelegate);
            var updatedFilenameDelegate = new GetFixedFilenameDelegate("updated", getJsonFilenameDelegate);

            var getUriFilenameDelegate = new GetUriFilenameDelegate();
            var getReportFilenameDelegate = new GetReportFilenameDelegate();
            var getValidationFilenameDelegate = new GetValidationFilenameDelegate();

            // index filenames

            var productsIndexDataController = new IndexDataController(
                collectionController,
                productsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var accountProductsIndexDataController = new IndexDataController(
                collectionController,
                accountProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var gameDetailsIndexDataController = new IndexDataController(
                collectionController,
                gameDetailsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var gameProductDataIndexDataController = new IndexDataController(
                collectionController,
                gameProductDataDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var apiProductsIndexDataController = new IndexDataController(
                collectionController,
                apiProductsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var productScreenshotsIndexDataController = new IndexDataController(
                collectionController,
                productScreenshotsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var productDownloadsIndexDataController = new IndexDataController(
                collectionController,
                productDownloadsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var productRoutesIndexDataController = new IndexDataController(
                collectionController,
                productRoutesDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
                statusController);

            var validationResultsIndexController = new IndexDataController(
                collectionController,
                validationResultsDirectoryDelegate,
                indexFilenameDelegate,
                serializedTransactionalStorageController,
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
                getProductIndexDelegate,
                collectionController,
                productsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var accountProductsDataController = new DataController<AccountProduct>(
                accountProductsIndexDataController,
                serializedStorageController,
                getAccountProductIndexDelegate,
                collectionController,
                accountProductsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var gameDetailsDataController = new DataController<GameDetails>(
                gameDetailsIndexDataController,
                serializedStorageController,
                getGameDetailsIndexDelegate,
                collectionController,
                gameDetailsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var gameProductDataController = new DataController<GameProductData>(
                gameProductDataIndexDataController,
                serializedStorageController,
                getGameProductDataIndexDelegate,
                collectionController,
                gameProductDataDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var apiProductsDataController = new DataController<ApiProduct>(
                apiProductsIndexDataController,
                serializedStorageController,
                getApiProductIndexDelegate,
                collectionController,
                apiProductsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                productScreenshotsIndexDataController,
                serializedStorageController,
                getProductScreenshotsIndexDelegate,
                collectionController,
                productScreenshotsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var productDownloadsDataController = new DataController<ProductDownloads>(
                productDownloadsIndexDataController,
                serializedStorageController,
                getProductDownloadsIndexDelegate,
                collectionController,
                productDownloadsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var productRoutesDataController = new DataController<ProductRoutes>(
                productRoutesIndexDataController,
                serializedStorageController,
                getProductRoutesIndexDelegate,
                collectionController,
                productRoutesDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            var validationResultsDataController = new DataController<ValidationResult>(
                validationResultsIndexController,
                serializedStorageController,
                getValidationResultIndexDelegate,
                collectionController,
                validationResultsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleBinController,
                statusController);

            #endregion

            #region Activity Controllers

            #region Authorize

            var usernamePasswordValidationDelegate = new UsernamePasswordValidationDelegate(consoleInputOutputController);
            var securityCodeValidationDelegate = new SecurityCodeValidationDelegate(consoleInputOutputController);

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

            var getProductUpdateUriByContextDelegate = new GetProductUpdateUriByContextDelegate();
            var productParameterGetQueryParametersDelegate = new ProductParameterGetQueryParametersDelegate();

            var getProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<ProductsPageResult>(
                Context.Products,
                getProductUpdateUriByContextDelegate,
                productParameterGetQueryParametersDelegate,
                requestPageController,
                stringMd5Controller,
                precomputedHashController,
                serializationController,
                statusController);

            var extractProductsDelegate = new ExtractProductsDelegate();

            var productsUpdateActivity = new PageResultUpdateActivity<ProductsPageResult, Product>(
                    Context.Products,
                    getProductsPageResultsAsyncDelegate,
                    extractProductsDelegate,
                    requestPageController,
                    productsDataController,
                    statusController);

            var getAccountProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<AccountProductsPageResult>(
                Context.AccountProducts,
                getProductUpdateUriByContextDelegate,
                productParameterGetQueryParametersDelegate,
                requestPageController,
                stringMd5Controller,
                precomputedHashController,
                serializationController,
                statusController);

            var extractAccountProductsDelegate = new ExtractAccountProductsDelegate();

            var selectNewUpdatedDelegate = new SelectNewUpdatedDelegate(
                accountProductsDataController,
                collectionController,
                updatedDataController,
                statusController);

            var accountProductsUpdateActivity = new PageResultUpdateActivity<AccountProductsPageResult, AccountProduct>(
                    Context.AccountProducts,
                    getAccountProductsPageResultsAsyncDelegate,
                    extractAccountProductsDelegate,
                    requestPageController,
                    accountProductsDataController,
                    statusController,
                    selectNewUpdatedDelegate);

            #endregion

            #region Update.Wishlisted

            var getDeserializedPageResultAsyncDelegate = new GetDeserializedGOGDataAsyncDelegate<ProductsPageResult>(networkController,
                gogDataExtractionController,
                serializationController);

            var wishlistedUpdateActivity = new WishlistedUpdateActivity(
                getDeserializedPageResultAsyncDelegate,
                wishlistedDataController,
                statusController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var getDeserializedGOGDataAsyncDelegate = new GetDeserializedGOGDataAsyncDelegate<GOGData>(networkController,
                gogDataExtractionController,
                serializationController);

            var getDeserializedGameProductDataAsyncDelegate = new GetDeserializedGameProductDataAsyncDelegate(
                getDeserializedGOGDataAsyncDelegate);

            var getProductUpdateIdentityDelegate = new GetProductUpdateIdentityDelegate();
            var getGameProductDataUpdateIdentityDelegate = new GetGameProductDataUpdateIdentityDelegate();
            var getAccountProductUpdateIdentityDelegate = new GetAccountProductUpdateIdentityDelegate();

            var fillGameDetailsGapsDelegate = new FillGameDetailsGapsDelegate();

            var userRequestedController = new UserRequestedController(args);

            // product update controllers

            var gameProductDataEnumerateGapsDelegate = new MasterDetailsEnumerateGapsDelegate<Product, GameProductData>(
                productsDataController,
                gameProductDataController);

            var userRequestedOrGameProductDataGapsAndUpdatedEnumerateDelegate = new UserRequestedOrDefaultEnumerateIdsDelegate(
                userRequestedController,
                gameProductDataEnumerateGapsDelegate,
                updatedDataController);

            var gameProductDataUpdateActivity = new MasterDetailProductUpdateActivity<Product, GameProductData>(
                Context.GameProductData,
                getProductUpdateUriByContextDelegate,
                userRequestedOrGameProductDataGapsAndUpdatedEnumerateDelegate,
                productsDataController,
                gameProductDataController,
                updatedDataController,
                getDeserializedGameProductDataAsyncDelegate,
                getGameProductDataUpdateIdentityDelegate,
                statusController);

            var getApiProductDelegate = new GetDeserializedGOGModelAsyncDelegate<ApiProduct>(
                networkController,
                serializationController);

            var apiProductEnumerateGapsDelegate = new MasterDetailsEnumerateGapsDelegate<Product, ApiProduct>(
                productsDataController,
                apiProductsDataController);

            var userRequestedOrApiProductGapsAndUpdatedEnumerateDelegate = new UserRequestedOrDefaultEnumerateIdsDelegate(
                userRequestedController,
                apiProductEnumerateGapsDelegate,
                updatedDataController);

            var apiProductUpdateActivity = new MasterDetailProductUpdateActivity<Product, ApiProduct>(
                Context.ApiProducts,
                getProductUpdateUriByContextDelegate,
                userRequestedOrApiProductGapsAndUpdatedEnumerateDelegate,
                productsDataController,
                apiProductsDataController,
                updatedDataController,
                getApiProductDelegate,
                getProductUpdateIdentityDelegate,
                statusController);

            var getDeserializedGameDetailsDelegate = new GetDeserializedGOGModelAsyncDelegate<GameDetails>(
                networkController,
                serializationController);

            var languageDownloadsContainmentController = new StringContainmentController(
                collectionController,
                Models.Separators.Separators.GameDetailsDownloadsStart,
                Models.Separators.Separators.GameDetailsDownloadsEnd);

            var gameDetailsLanguagesExtractionController = new GameDetailsLanguagesExtractionController();
            var gameDetailsDownloadsExtractionController = new GameDetailsDownloadsExtractionController();

            var sanitizationController = new SanitizationController();

            var extractOperatingSystemsDownloadsDelegate = new ExtractOperatingSystemsDownloadsDelegate(
                sanitizationController,
                languageController);

            var getDeserializedGameDetailsAsyncDelegate = new GetDeserializedGameDetailsAsyncDelegate(
                networkController,
                serializationController,
                languageController,
                languageDownloadsContainmentController,
                gameDetailsLanguagesExtractionController,
                gameDetailsDownloadsExtractionController,
                sanitizationController,
                extractOperatingSystemsDownloadsDelegate);

            var gameDetailsEnumerateGapsDelegate = new MasterDetailsEnumerateGapsDelegate<AccountProduct, GameDetails>(
                accountProductsDataController,
                gameDetailsDataController);

            var userRequestedOrGameDetailsGapsAndUpdatedEnumerateDelegate = new UserRequestedOrDefaultEnumerateIdsDelegate(
                userRequestedController,
                gameDetailsEnumerateGapsDelegate,
                updatedDataController);

            var gameDetailsUpdateActivity = new MasterDetailProductUpdateActivity<AccountProduct, GameDetails>(
                Context.GameDetails,
                getProductUpdateUriByContextDelegate,
                userRequestedOrGameDetailsGapsAndUpdatedEnumerateDelegate,
                accountProductsDataController,
                gameDetailsDataController,
                updatedDataController,
                getDeserializedGameDetailsAsyncDelegate,
                getAccountProductUpdateIdentityDelegate,
                statusController,
                fillGameDetailsGapsDelegate);

            #endregion

            #region Update.Screenshots

            var updateScreenshotsAsyncDelegate = new UpdateScreenshotsAsyncDelegate(
                getProductUpdateUriByContextDelegate,
                screenshotsDataController,
                networkController,
                screenshotExtractionController,
                statusController);

            var updateScreenshotsActivity = new UpdateScreenshotsActivity(
                productsDataController,
                productScreenshotsIndexDataController,
                updateScreenshotsAsyncDelegate,
                statusController);

            #endregion

            // dependencies for download controllers

            var getProductImageUriDelegate = new GetProductImageUriDelegate();
            var getAccountProductImageUriDelegate = new GetAccountProductImageUriDelegate();

            var userRequestedOrUpdatedEnumerateDelegate = new UserRequestedOrDefaultEnumerateIdsDelegate(
                userRequestedController,
                updatedDataController);

            var getProductsImagesDownloadSourcesAsyncDelegate = new GetProductCoreImagesDownloadSourcesAsyncDelegate<Product>(
                userRequestedOrUpdatedEnumerateDelegate,
                productsDataController,
                expandImageUriDelegate,
                getProductImageUriDelegate,
                statusController);

            var getAccountProductsImagesDownloadSourcesAsyncDelegate = new GetProductCoreImagesDownloadSourcesAsyncDelegate<AccountProduct>(
                userRequestedOrUpdatedEnumerateDelegate,
                accountProductsDataController,
                expandImageUriDelegate,
                getAccountProductImageUriDelegate,
                statusController);

            var getScreenshotsDownloadSourcesAsyncDelegate = new GetScreenshotsDownloadSourcesAsyncDelegate(
                screenshotsDataController,
                expandScreenshotUriDelegate,
                screenshotsDirectoryDelegate,
                fileController,
                statusController);

            var routingController = new RoutingController(
                productRoutesDataController,
                statusController);

            var enumerateGameDetailsManualUrlsAsyncDelegate = new EnumerateGameDetailsManualUrlsAsyncDelegate(
                settingsController,
                gameDetailsDataController);

            var enumerateGameDetailsDirectoriesAsyncDelegate = new EnumerateGameDetailsDirectoriesAsyncDelegate(
                enumerateGameDetailsManualUrlsAsyncDelegate,
                productFilesDirectoryDelegate);

            var enumerateGameDetailsFilesAsyncDelegate = new EnumerateGameDetailsFilesAsyncDelegate(
                enumerateGameDetailsManualUrlsAsyncDelegate,
                routingController,
                productFilesDirectoryDelegate,
                getUriFilenameDelegate,
                statusController);

            // product files are driven through gameDetails manual urls
            // so this sources enumerates all manual urls for all updated game details
            var getManualUrlDownloadSourcesAsyncDelegate = new GetManualUrlDownloadSourcesAsyncDelegate(
                updatedDataController,
                gameDetailsDataController,
                enumerateGameDetailsManualUrlsAsyncDelegate,
                statusController);

            // schedule download controllers

            var updateProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Context.ProductsImages,
                getProductsImagesDownloadSourcesAsyncDelegate,
                imagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateAccountProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Context.AccountProductsImages,
                getAccountProductsImagesDownloadSourcesAsyncDelegate,
                imagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateScreenshotsDownloadsActivity = new UpdateDownloadsActivity(
                Context.Screenshots,
                getScreenshotsDownloadSourcesAsyncDelegate,
                screenshotsDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateProductFilesDownloadsActivity = new UpdateDownloadsActivity(
                Context.ProductsFiles,
                getManualUrlDownloadSourcesAsyncDelegate,
                productFilesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            // downloads processing

            var uriSansSessionExtractionController = new UriSansSessionExtractionController();

            var validationExpectedDelegate = new ValidationExpectedDelegate();

            var getValidationUriDelegate = new GetValidationUriDelegate(
                getValidationFilenameDelegate,
                uriSansSessionExtractionController);

            var validationFileEnumerateDelegate = new ValidationFileEnumerateDelegate(
                validationDirectoryDelegate,
                getValidationFilenameDelegate);

            var downloadValidationFileAsyncDelegate = new DownloadValidationFileAsyncDelegate(
                uriSansSessionExtractionController,
                validationExpectedDelegate,
                validationFileEnumerateDelegate,
                validationDirectoryDelegate,
                getValidationUriDelegate,
                fileController,
                fileDownloadController,
                statusController);

            var downloadManualUrlFileAsyncDelegate = new DownloadManualUrlFileAsyncDelegate(
                networkController,
                uriSansSessionExtractionController,
                routingController,
                fileDownloadController,
                downloadValidationFileAsyncDelegate,
                statusController);

            var productsImagesDownloadActivity = new DownloadFilesActivity(
                Context.ProductsImages,
                productDownloadsDataController,
                null,
                fileDownloadController,
                statusController);

            var accountProductsImagesDownloadActivity = new DownloadFilesActivity(
                Context.AccountProductsImages,
                productDownloadsDataController,
                null,
                fileDownloadController,
                statusController);

            var screenshotsDownloadActivity = new DownloadFilesActivity(
                Context.Screenshots,
                productDownloadsDataController,
                null,
                fileDownloadController,
                statusController);

            var productFilesDownloadActivity = new DownloadFilesActivity(
                Context.ProductsFiles,
                productDownloadsDataController,
                downloadManualUrlFileAsyncDelegate,
                null,
                statusController);

            // validation controllers

            var validationResultController = new ValidationResultController();

            var fileMd5Controller = new FileMd5Controller(
                storageController,
                stringMd5Controller);

            var dataFileValidateDelegate = new DataFileValidateDelegate(
                fileMd5Controller,
                statusController);

            var productFileValidationController = new FileValidationController(
                validationExpectedDelegate,
                fileController,
                streamController,
                bytesMd5Controller,
                validationResultController,
                statusController);

            var validateProductFilesActivity = new ValidateProductFilesActivity(
                productFilesDirectoryDelegate,
                getUriFilenameDelegate,
                validationFileEnumerateDelegate,
                productFileValidationController,
                validationResultsDataController,
                gameDetailsDataController,
                enumerateGameDetailsManualUrlsAsyncDelegate,
                userRequestedOrUpdatedEnumerateDelegate,
                routingController,
                statusController);

            var validateDataActivity = new ValidateDataActivity(
                precomputedHashController,
                fileController,
                dataFileValidateDelegate,
                statusController);

            #region Repair

            var repairActivity = new RepairActivity(
                validationResultsDataController,
                validationResultController,
                statusController);

            #endregion

            #region Cleanup

            var enumerateAllGameDetailsDirectoriesAsyncDelegate = new EnumerateAllGameDetailsDirectoriesAsyncDelegate(
                gameDetailsDataController,
                enumerateGameDetailsDirectoriesAsyncDelegate,
                statusController);

            var productFilesDirectoriesEnumerateAllDelegate = new ProductFilesDirectoriesEnumerateAllDelegate(
                productFilesBaseDirectoryDelegate,
                directoryController,
                statusController);

            var directoryFilesEnumerateDelegate = new DirectoryFilesEnumerateDelegate(directoryController);

            var directoryCleanupActivity = new CleanupActivity(
                Context.Directories,
                enumerateAllGameDetailsDirectoriesAsyncDelegate, // expected items (directories for gameDetails)
                productFilesDirectoriesEnumerateAllDelegate, // actual items (directories in productFiles)
                directoryFilesEnumerateDelegate, // detailed items (files in directory)
                validationFileEnumerateDelegate, // supplementary items (validation files)
                recycleBinController,
                directoryController,
                statusController);

            var enumerateAllUpdatedGameDetailsManualUrlFilesAsyncDelegate = new EnumerateAllUpdatedGameDetailsManualUrlFilesAsyncDelegate(
                updatedDataController,
                gameDetailsDataController,
                enumerateGameDetailsFilesAsyncDelegate,
                statusController);

            var enumerateAllUpdatedProductFilesAsyncDelegate = new EnumerateAllUpdatedProductFilesAsyncDelegate(
                updatedDataController,
                gameDetailsDataController,
                enumerateGameDetailsDirectoriesAsyncDelegate,
                directoryController,
                statusController);

            var passthroughEnumerateDelegate = new PassthroughEnumerateDelegate();

            var fileCleanupActivity = new CleanupActivity(
                Context.Files,
                enumerateAllUpdatedGameDetailsManualUrlFilesAsyncDelegate, // expected items (files for updated gameDetails)
                enumerateAllUpdatedProductFilesAsyncDelegate, // actual items (updated product files)
                passthroughEnumerateDelegate, // detailed items (passthrough)
                validationFileEnumerateDelegate, // supplementary items (validation files)
                recycleBinController,
                directoryController,
                statusController);

            var cleanupUpdatedActivity = new CleanupUpdatedActivity(
                updatedDataController,
                statusController);

            #endregion

            var aliasController = new AliasController(ActivityContext.Aliases);
            var whitelistController = new WhitelistController(ActivityContext.Whitelist);
            var prerequisitesController = new PrerequisiteController(ActivityContext.Prerequisites);
            var supplementaryController = new SupplementaryController(ActivityContext.Supplementary);

            var activityContextController = new ActivityContextController(
                aliasController,
                whitelistController,
                prerequisitesController,
                supplementaryController);

            #region Help

            var helpActivity = new HelpActivity(
                activityContextController,
                statusController);

            #endregion

            #region Report Task Status 

            var reportFilePresentationController = new FilePresentationController(
                reportDirectoryDelegate,
                getReportFilenameDelegate,
                streamController);

            var fileNotifyStatusViewUpdateController = new NotifyStatusViewUpdateController(
                getStatusViewUpdateDelegate,
                reportFilePresentationController);

            var reportActivity = new ReportActivity(
                fileNotifyStatusViewUpdateController,
                statusController);

            #endregion

            #region List

            var listUpdatedActivity = new ListUpdatedActivity(
                updatedDataController,
                accountProductsDataController,
                statusController,
                consoleController);

            #endregion

            #endregion

            #region Activity Context To Activity Controllers Mapping

            var activityContextToActivityControllerMap = new Dictionary<(Activity, Context), IActivity>()
            {
                { (Activity.Validate, Context.Settings), validateSettingsActivity },
                { (Activity.Authorize, Context.None), authorizeActivity },
                { (Activity.UpdateData, Context.Products), productsUpdateActivity },
                { (Activity.UpdateData, Context.AccountProducts), accountProductsUpdateActivity },
                { (Activity.UpdateData, Context.Wishlist), wishlistedUpdateActivity },
                { (Activity.UpdateData, Context.GameProductData), gameProductDataUpdateActivity },
                { (Activity.UpdateData, Context.ApiProducts), apiProductUpdateActivity },
                { (Activity.UpdateData, Context.GameDetails), gameDetailsUpdateActivity },
                { (Activity.UpdateData, Context.Screenshots), updateScreenshotsActivity },
                { (Activity.UpdateDownloads, Context.ProductsImages), updateProductsImagesDownloadsActivity },
                { (Activity.UpdateDownloads, Context.AccountProductsImages), updateAccountProductsImagesDownloadsActivity },
                { (Activity.UpdateDownloads, Context.Screenshots), updateScreenshotsDownloadsActivity },
                { (Activity.UpdateDownloads, Context.ProductsFiles), updateProductFilesDownloadsActivity },
                { (Activity.Download, Context.ProductsImages), productsImagesDownloadActivity },
                { (Activity.Download, Context.AccountProductsImages), accountProductsImagesDownloadActivity },
                { (Activity.Download, Context.Screenshots), screenshotsDownloadActivity },
                { (Activity.Download, Context.ProductsFiles), productFilesDownloadActivity },
                { (Activity.Validate, Context.ProductsFiles), validateProductFilesActivity },
                { (Activity.Validate, Context.Data), validateDataActivity },
                { (Activity.Repair, Context.ProductsFiles), repairActivity },
                { (Activity.Cleanup, Context.Directories), directoryCleanupActivity },
                { (Activity.Cleanup, Context.Files), fileCleanupActivity },
                { (Activity.Cleanup, Context.Updated), cleanupUpdatedActivity },
                { (Activity.Report, Context.None), reportActivity },
                { (Activity.List, Context.Updated), listUpdatedActivity },
                { (Activity.Help, Context.None), helpActivity }
            };



            var activityContextQueue = activityContextController.GetQueue(args);
            var commandLineParameters = activityContextController.GetParameters(args).ToArray();

            #endregion

            #region Core Activities Loop

            foreach (var activityContext in activityContextQueue)
            {
                if (!activityContextToActivityControllerMap.ContainsKey(activityContext))
                {
                    await statusController.WarnAsync(
                        applicationStatus,
                        activityContextController.ToString(activityContext) + " is not mapped to an Activity.");
                    continue;
                }

                var activity = activityContextToActivityControllerMap[activityContext];
                try
                {
                    await activity.ProcessActivityAsync(applicationStatus);
                }
                catch (AggregateException ex)
                {
                    var exceptionTreeToEnumerableController = new ExceptionTreeToEnumerableController();

                    List<string> errorMessages = new List<string>();
                    foreach (var innerException in exceptionTreeToEnumerableController.ToEnumerable(ex))
                        errorMessages.Add(innerException.Message);

                    var combinedErrorMessages = string.Join(Models.Separators.Separators.Common.Comma, errorMessages);

                    await statusController.FailAsync(applicationStatus, combinedErrorMessages);

                    var failureDumpUri = "failureDump.json";
                    await serializedStorageController.SerializePushAsync(failureDumpUri, applicationStatus, applicationStatus);

                    await consoleInputOutputController.OutputOnRefreshAsync(
                        "GoodOfflineGames.exe has encountered fatal error(s): " +
                        combinedErrorMessages +
                        $".\nPlease refer to {failureDumpUri} for further details.\n" +
                        "Press ENTER to close the window...");

                    consoleController.ReadLine();

                    return;
                }
            }

            #endregion

            // TODO: Present session results

            if (applicationStatus.SummaryResults != null)
            {
                foreach (var line in applicationStatus.SummaryResults)
                    await consoleInputOutputController.OutputOnRefreshAsync(string.Join(" ", applicationStatus.SummaryResults));
            }
            else
            {
                await consoleInputOutputController.OutputContinuousAsync(string.Empty, "All tasks are complete. Press ENTER to exit...");
            }

            consoleController.ReadLine();
        }
    }
}
