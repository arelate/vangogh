#region Using

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Delegates.Convert;
using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.Format.Numbers;
using Delegates.Format.Uri;
using Delegates.Format.Text;
using Delegates.Format.Status;
using Delegates.Confirm;
using Delegates.GetQueryParameters;
using Delegates.Recycle;
using Delegates.Correct;
using Delegates.Constrain;
using Delegates.Itemize;
using Delegates.Replace;
using Delegates.EnumerateIds;
using Delegates.Download;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Uri;
using Controllers.Network;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.Extraction;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Settings;
using Controllers.Cookies;
using Controllers.Validation;
using Controllers.ValidationResult;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Presentation;
using Controllers.Routing;
using Controllers.Status;
using Controllers.Hash;
using Controllers.Template;
using Controllers.ViewModel;
using Controllers.ViewUpdates;
using Controllers.ActivityContext;
using Controllers.InputOutput;

using Interfaces.Activity;
using Interfaces.Extraction;
using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;
using Interfaces.Status;

using GOG.Models;

using GOG.Delegates.GetPageResults;
using GOG.Delegates.FillGaps;
using GOG.Delegates.GetDownloadSources;
using GOG.Delegates.GetUpdateIdentity;
using GOG.Delegates.DownloadProductFile;
using GOG.Delegates.GetImageUri;
using GOG.Delegates.UpdateScreenshots;
using GOG.Delegates.GetDeserialized;
using GOG.Delegates.Itemize;
using GOG.Delegates.GetUpdateUri;
using GOG.Delegates.Format;
using GOG.Delegates.RequestPage;

using GOG.Controllers.NewUpdatedSelection;
using GOG.Controllers.Authorization;

using GOG.Activities.Help;
using GOG.Activities.CorrectSettings;
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
            var formatTextToFitConsoleWindowDelegate = new FormatTextToFitConsoleWindowDelegate(consoleController);

            var consoleInputOutputController = new ConsoleInputOutputController(
                formatTextToFitConsoleWindowDelegate,
                consoleController);

            var formatBytesDelegate = new FormatBytesDelegate();
            var formatSecondsDelegate = new FormatSecondsDelegate();

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

            var itemizeStatusChildrenDelegate = new ItemizeStatusChildrenDelegate();

            var statusTreeToEnumerableController = new ConvertTreeToEnumerableDelegate<IStatus>(itemizeStatusChildrenDelegate);

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

            var formatRemainingTimeAtSpeedDelegate = new FormatRemainingTimeAtSpeedDelegate();

            var statusAppViewModelDelegate = new StatusAppViewModelDelegate(
                formatRemainingTimeAtSpeedDelegate,
                formatBytesDelegate,
                formatSecondsDelegate);

            var statusReportViewModelDelegate = new StatusReportViewModelDelegate(
                formatBytesDelegate,
                formatSecondsDelegate);

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

            var constrainExecutionAsyncDelegate = new ConstrainExecutionAsyncDelegate(
                statusController,
                formatSecondsDelegate);

            var constrainRequestRateAsyncDelegate = new ConstrainRequestRateAsyncDelegate(
                constrainExecutionAsyncDelegate,
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
                constrainRequestRateAsyncDelegate);

            var downloadFromResponseAsyncDelegate = new DownloadFromResponseAsyncDelegate(
                networkController,
                streamController,
                fileController,
                statusController);

            var downloadFromUriAsyncDelegate = new DownloadFromUriAsyncDelegate(
                networkController,
                downloadFromResponseAsyncDelegate,
                statusController);

            var requestPageAsyncDelegate = new RequestPageAsyncDelegate(
                networkController);

            var languageController = new LanguageController();

            var loginIdExtractionController = new LoginIdExtractionController();
            var loginUsernameExtractionController = new LoginUsernameExtractionController();
            var loginUnderscoreTokenExtractionController = new LoginTokenExtractionController();
            var secondStepAuthenticationUnderscoreTokenExtractionController = new SecondStepAuthenticationTokenExtractionController();

            var gogDataExtractionController = new GOGDataExtractionController();
            var screenshotExtractionController = new ScreenshotExtractionController();

            var formatImagesUriDelegate = new FormatImagesUriDelegate();
            var formatScreenshotsUriDelegate = new FormatScreenshotsUriDelegate();

            #endregion

            #region Data Controllers

            // Data controllers for products, game details, game product data, etc.

            var convertProductToIndexDelegate = new ConvertProductCoreToIndexDelegate<Product>();
            var convertAccountProductToIndexDelegate = new ConvertProductCoreToIndexDelegate<AccountProduct>();
            var convertGameDetailsToIndexDelegate = new ConvertProductCoreToIndexDelegate<GameDetails>();
            var convertGameProductDataToIndexDelegate = new ConvertProductCoreToIndexDelegate<GameProductData>();
            var convertApiProductToIndexDelegate = new ConvertProductCoreToIndexDelegate<ApiProduct>();
            var convertProductScreenshotsToIndexDelegate = new ConvertProductCoreToIndexDelegate<ProductScreenshots>();
            var convertProductDownloadsToIndexDelegate = new ConvertProductCoreToIndexDelegate<ProductDownloads>();
            var convertProductRoutesToIndexDelegate = new ConvertProductCoreToIndexDelegate<ProductRoutes>();
            var convertValidationResultToIndexDelegate = new ConvertProductCoreToIndexDelegate<ValidationResult>();

            // directories

            var settingsFilenameDelegate = new GetFixedFilenameDelegate("settings", getJsonFilenameDelegate);

            var settingsController = new SettingsController(
                settingsFilenameDelegate,
                serializedStorageController,
                statusController);

            var correctDownloadsLanguagesAsyncDelegate = new CorrectDownloadsLanguagesAsyncDelegate(languageController);
            var correctDownloadsOperatingSystemsAsyncDelegate = new CorrectDownloadsOperatingSystemsAsyncDelegate();
            var correctSettingsDirectoriesAsyncDelegate = new CorrectSettingsDirectoriesAsyncDelegate();

            var correctSettingsActivity = new CorrectSettingsActivity(
                settingsController,
                correctDownloadsLanguagesAsyncDelegate,
                correctDownloadsOperatingSystemsAsyncDelegate,
                correctSettingsDirectoriesAsyncDelegate,
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

            var recycleDelegate = new RecycleDelegate(
                recycleBinDirectoryDelegate,
                fileController,
                directoryController);

            var productsDataController = new DataController<Product>(
                productsIndexDataController,
                serializedStorageController,
                convertProductToIndexDelegate,
                collectionController,
                productsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var accountProductsDataController = new DataController<AccountProduct>(
                accountProductsIndexDataController,
                serializedStorageController,
                convertAccountProductToIndexDelegate,
                collectionController,
                accountProductsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var gameDetailsDataController = new DataController<GameDetails>(
                gameDetailsIndexDataController,
                serializedStorageController,
                convertGameDetailsToIndexDelegate,
                collectionController,
                gameDetailsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var gameProductDataController = new DataController<GameProductData>(
                gameProductDataIndexDataController,
                serializedStorageController,
                convertGameProductDataToIndexDelegate,
                collectionController,
                gameProductDataDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var apiProductsDataController = new DataController<ApiProduct>(
                apiProductsIndexDataController,
                serializedStorageController,
                convertApiProductToIndexDelegate,
                collectionController,
                apiProductsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                productScreenshotsIndexDataController,
                serializedStorageController,
                convertProductScreenshotsToIndexDelegate,
                collectionController,
                productScreenshotsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var productDownloadsDataController = new DataController<ProductDownloads>(
                productDownloadsIndexDataController,
                serializedStorageController,
                convertProductDownloadsToIndexDelegate,
                collectionController,
                productDownloadsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var productRoutesDataController = new DataController<ProductRoutes>(
                productRoutesIndexDataController,
                serializedStorageController,
                convertProductRoutesToIndexDelegate,
                collectionController,
                productRoutesDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            var validationResultsDataController = new DataController<ValidationResult>(
                validationResultsIndexController,
                serializedStorageController,
                convertValidationResultToIndexDelegate,
                collectionController,
                validationResultsDirectoryDelegate,
                getJsonFilenameDelegate,
                recycleDelegate,
                statusController);

            #endregion

            #region Activity Controllers

            #region Authorize

            var correctUsernamePasswordAsyncDelegate = new CorrectUsernamePasswordAsyncDelegate(
                consoleInputOutputController);
            var correctSecurityCodeAsyncDelegate = new CorrectSecurityCodeAsyncDelegate(
                consoleInputOutputController);

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
                correctUsernamePasswordAsyncDelegate,
                correctSecurityCodeAsyncDelegate,
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
            var getQueryParametersForProductContextDelegate = new GetQueryParametersForProductContextDelegate();

            var getProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<ProductsPageResult>(
                Context.Products,
                getProductUpdateUriByContextDelegate,
                getQueryParametersForProductContextDelegate,
                requestPageAsyncDelegate,
                stringMd5Controller,
                precomputedHashController,
                serializationController,
                statusController);

            var itemizeProductsPageResultProductsDelegate = new ItemizeProductsPageResultProductsDelegate();

            var productsUpdateActivity = new PageResultUpdateActivity<ProductsPageResult, Product>(
                    Context.Products,
                    getProductsPageResultsAsyncDelegate,
                    itemizeProductsPageResultProductsDelegate,
                    //requestPageAsyncDelegate,
                    productsDataController,
                    statusController);

            var getAccountProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<AccountProductsPageResult>(
                Context.AccountProducts,
                getProductUpdateUriByContextDelegate,
                getQueryParametersForProductContextDelegate,
                requestPageAsyncDelegate,
                stringMd5Controller,
                precomputedHashController,
                serializationController,
                statusController);

            var itemizeAccountProductsPageResultProductsDelegate = new ItemizeAccountProductsPageResultProductsDelegate();

            var selectNewUpdatedDelegate = new SelectNewUpdatedDelegate(
                accountProductsDataController,
                collectionController,
                updatedDataController,
                statusController);

            var accountProductsUpdateActivity = new PageResultUpdateActivity<AccountProductsPageResult, AccountProduct>(
                    Context.AccountProducts,
                    getAccountProductsPageResultsAsyncDelegate,
                    itemizeAccountProductsPageResultProductsDelegate,
                    //requestPageAsyncDelegate,
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

            var itemizeAllUserRequestedIdsAsyncDelegate = new ItemizeAllUserRequestedIdsAsyncDelegate(args);

            // product update controllers

            var itemizeAllGameProductDataGapsAsyncDelegatepsDelegate = new ItemizeAllMasterDetailsGapsAsyncDelegate<Product, GameProductData>(
                productsDataController,
                gameProductDataController);

            var itemizeAllUserRequestedIdsOrDefaultAsyncDelegate = new ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
                itemizeAllUserRequestedIdsAsyncDelegate,
                itemizeAllGameProductDataGapsAsyncDelegatepsDelegate,
                updatedDataController);

            var gameProductDataUpdateActivity = new MasterDetailProductUpdateActivity<Product, GameProductData>(
                Context.GameProductData,
                getProductUpdateUriByContextDelegate,
                itemizeAllUserRequestedIdsOrDefaultAsyncDelegate,
                productsDataController,
                gameProductDataController,
                updatedDataController,
                getDeserializedGameProductDataAsyncDelegate,
                getGameProductDataUpdateIdentityDelegate,
                statusController);

            var getApiProductDelegate = new GetDeserializedGOGModelAsyncDelegate<ApiProduct>(
                networkController,
                serializationController);

            var itemizeAllApiProductsGapsAsyncDelegate = new ItemizeAllMasterDetailsGapsAsyncDelegate<Product, ApiProduct>(
                productsDataController,
                apiProductsDataController);

            var itemizeAllUserRequestedOrApiProductGapsAndUpdatedDelegate = new ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
                itemizeAllUserRequestedIdsAsyncDelegate,
                itemizeAllApiProductsGapsAsyncDelegate,
                updatedDataController);

            var apiProductUpdateActivity = new MasterDetailProductUpdateActivity<Product, ApiProduct>(
                Context.ApiProducts,
                getProductUpdateUriByContextDelegate,
                itemizeAllUserRequestedOrApiProductGapsAndUpdatedDelegate,
                productsDataController,
                apiProductsDataController,
                updatedDataController,
                getApiProductDelegate,
                getProductUpdateIdentityDelegate,
                statusController);

            var getDeserializedGameDetailsDelegate = new GetDeserializedGOGModelAsyncDelegate<GameDetails>(
                networkController,
                serializationController);

            var confirmStringContainsLanguageDownloadsDelegate = new ConfirmStringMatchesAllDelegate(
                collectionController,
                Models.Separators.Separators.GameDetailsDownloadsStart,
                Models.Separators.Separators.GameDetailsDownloadsEnd);

            var replaceMultipleStringsDelegate = new ReplaceMultipleStringsDelegate();

            var itemizeDownloadLanguagesDelegate = new ItemizeDownloadLanguagesDelegate(
                languageController,
                replaceMultipleStringsDelegate);

            var itemizeGameDetailsDownloadsDelegate = new ItemizeGameDetailsDownloadsDelegate();

            var formatDownloadLanguagesDelegate = new FormatDownloadLanguageDelegate(
                replaceMultipleStringsDelegate);

            var convertOperatingSystemsDownloads2DArrayToArrayDelegate = new Convert2DArrayToArrayDelegate<OperatingSystemsDownloads>();

            var getDeserializedGameDetailsAsyncDelegate = new GetDeserializedGameDetailsAsyncDelegate(
                networkController,
                serializationController,
                languageController,
                formatDownloadLanguagesDelegate,
                confirmStringContainsLanguageDownloadsDelegate,
                itemizeDownloadLanguagesDelegate,
                itemizeGameDetailsDownloadsDelegate,
                replaceMultipleStringsDelegate,
                convertOperatingSystemsDownloads2DArrayToArrayDelegate,
                collectionController);

            var itemizeAllGameDetailsGapsAsyncDelegate = new ItemizeAllMasterDetailsGapsAsyncDelegate<AccountProduct, GameDetails>(
                accountProductsDataController,
                gameDetailsDataController);

            var itemizeAllUserRequestedOrDefaultAsyncDelegate = new ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
                itemizeAllUserRequestedIdsAsyncDelegate,
                itemizeAllGameDetailsGapsAsyncDelegate,
                updatedDataController);

            var gameDetailsUpdateActivity = new MasterDetailProductUpdateActivity<AccountProduct, GameDetails>(
                Context.GameDetails,
                getProductUpdateUriByContextDelegate,
                itemizeAllUserRequestedOrDefaultAsyncDelegate,
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

            var itemizeAllUserRequestedIdsOrUpdatedAsyncDelegate = new ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
                itemizeAllUserRequestedIdsAsyncDelegate,
                updatedDataController);

            var getProductsImagesDownloadSourcesAsyncDelegate = new GetProductCoreImagesDownloadSourcesAsyncDelegate<Product>(
                itemizeAllUserRequestedIdsOrUpdatedAsyncDelegate,
                productsDataController,
                formatImagesUriDelegate,
                getProductImageUriDelegate,
                statusController);

            var getAccountProductsImagesDownloadSourcesAsyncDelegate = new GetProductCoreImagesDownloadSourcesAsyncDelegate<AccountProduct>(
                itemizeAllUserRequestedIdsOrUpdatedAsyncDelegate,
                accountProductsDataController,
                formatImagesUriDelegate,
                getAccountProductImageUriDelegate,
                statusController);

            var getScreenshotsDownloadSourcesAsyncDelegate = new GetScreenshotsDownloadSourcesAsyncDelegate(
                screenshotsDataController,
                formatScreenshotsUriDelegate,
                screenshotsDirectoryDelegate,
                fileController,
                statusController);

            var routingController = new RoutingController(
                productRoutesDataController,
                statusController);

            var itemizeGameDetailsManualUrlsAsyncDelegate = new ItemizeGameDetailsManualUrlsAsyncDelegate(
                settingsController,
                gameDetailsDataController);

            var itemizeGameDetailsDirectoriesAsyncDelegate = new ItemizeGameDetailsDirectoriesAsyncDelegate(
                itemizeGameDetailsManualUrlsAsyncDelegate,
                productFilesDirectoryDelegate);

            var itemizeGameDetailsFilesAsyncDelegate = new ItemizeGameDetailsFilesAsyncDelegate(
                itemizeGameDetailsManualUrlsAsyncDelegate,
                routingController,
                productFilesDirectoryDelegate,
                getUriFilenameDelegate,
                statusController);

            // product files are driven through gameDetails manual urls
            // so this sources enumerates all manual urls for all updated game details
            var getManualUrlDownloadSourcesAsyncDelegate = new GetManualUrlDownloadSourcesAsyncDelegate(
                updatedDataController,
                gameDetailsDataController,
                itemizeGameDetailsManualUrlsAsyncDelegate,
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

            var formatUriRemoveSessionDelegate = new FormatUriRemoveSessionDelegate();

            var confirmValidationExpectedDelegate = new ConfirmValidationExpectedDelegate();

            var formatValidationUriDelegate = new FormatValidationUriDelegate(
                getValidationFilenameDelegate,
                formatUriRemoveSessionDelegate);

            var formatValidationFileDelegate = new FormatValidationFileDelegate(
                validationDirectoryDelegate,
                getValidationFilenameDelegate);

            var downloadValidationFileAsyncDelegate = new DownloadValidationFileAsyncDelegate(
                formatUriRemoveSessionDelegate,
                confirmValidationExpectedDelegate,
                formatValidationFileDelegate,
                validationDirectoryDelegate,
                formatValidationUriDelegate,
                fileController,
                downloadFromUriAsyncDelegate,
                statusController);

            var downloadManualUrlFileAsyncDelegate = new DownloadManualUrlFileAsyncDelegate(
                networkController,
                formatUriRemoveSessionDelegate,
                routingController,
                downloadFromResponseAsyncDelegate,
                downloadValidationFileAsyncDelegate,
                statusController);

            var downloadProductImageAsyncDelegate = new DownloadProductImageAsyncDelegate(downloadFromUriAsyncDelegate);

            var productsImagesDownloadActivity = new DownloadFilesActivity(
                Context.ProductsImages,
                productDownloadsDataController,
                downloadProductImageAsyncDelegate,
                statusController);

            var accountProductsImagesDownloadActivity = new DownloadFilesActivity(
                Context.AccountProductsImages,
                productDownloadsDataController,
                downloadProductImageAsyncDelegate,
                statusController);

            var screenshotsDownloadActivity = new DownloadFilesActivity(
                Context.Screenshots,
                productDownloadsDataController,
                downloadProductImageAsyncDelegate,
                statusController);

            var productFilesDownloadActivity = new DownloadFilesActivity(
                Context.ProductsFiles,
                productDownloadsDataController,
                downloadManualUrlFileAsyncDelegate,
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
                confirmValidationExpectedDelegate,
                fileController,
                streamController,
                bytesMd5Controller,
                validationResultController,
                statusController);

            var validateProductFilesActivity = new ValidateProductFilesActivity(
                productFilesDirectoryDelegate,
                getUriFilenameDelegate,
                formatValidationFileDelegate,
                productFileValidationController,
                validationResultsDataController,
                gameDetailsDataController,
                itemizeGameDetailsManualUrlsAsyncDelegate,
                itemizeAllUserRequestedIdsOrUpdatedAsyncDelegate,
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

            var itemizeAllGameDetailsDirectoriesAsyncDelegate = new ItemizeAllGameDetailsDirectoriesAsyncDelegate(
                gameDetailsDataController,
                itemizeGameDetailsDirectoriesAsyncDelegate,
                statusController);

            var itemizeAllProductFilesDirectoriesAsyncDelegate = new ItemizeAllProductFilesDirectoriesAsyncDelegate(
                productFilesBaseDirectoryDelegate,
                directoryController,
                statusController);

            var itemizeDirectoryFilesDelegate = new ItemizeDirectoryFilesDelegate(directoryController);

            var directoryCleanupActivity = new CleanupActivity(
                Context.Directories,
                itemizeAllGameDetailsDirectoriesAsyncDelegate, // expected items (directories for gameDetails)
                itemizeAllProductFilesDirectoriesAsyncDelegate, // actual items (directories in productFiles)
                itemizeDirectoryFilesDelegate, // detailed items (files in directory)
                formatValidationFileDelegate, // supplementary items (validation files)
                recycleDelegate,
                directoryController,
                statusController);

            var itemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate = 
                new ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate(
                    updatedDataController,
                    gameDetailsDataController,
                    itemizeGameDetailsFilesAsyncDelegate,
                    statusController);

            var itemizeAllUpdatedProductFilesAsyncDelegate = 
                new ItemizeAllUpdatedProductFilesAsyncDelegate(
                    updatedDataController,
                    gameDetailsDataController,
                    itemizeGameDetailsDirectoriesAsyncDelegate,
                    directoryController,
                    statusController);

            var itemizePassthroughDelegate = new ItemizePassthroughDelegate();

            var fileCleanupActivity = new CleanupActivity(
                Context.Files,
                itemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate, // expected items (files for updated gameDetails)
                itemizeAllUpdatedProductFilesAsyncDelegate, // actual items (updated product files)
                itemizePassthroughDelegate, // detailed items (passthrough)
                formatValidationFileDelegate, // supplementary items (validation files)
                recycleDelegate,
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
                { (Activity.Correct, Context.Settings), correctSettingsActivity },
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
                    var itemizeInnerExceptionsDelegate = new ItemizeInnerExceptionsDelegate();
                    var convertTreeToEnumerableDelegate = new ConvertTreeToEnumerableDelegate<Exception>(itemizeInnerExceptionsDelegate);

                    List<string> errorMessages = new List<string>();
                    foreach (var innerException in convertTreeToEnumerableDelegate.Convert(ex))
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
