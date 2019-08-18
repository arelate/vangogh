#region Using

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Delegates.Convert;
using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.GetPath;
using Delegates.Format.Numbers;
using Delegates.Format.Uri;
using Delegates.Format.Text;
using Delegates.Format.Status;
using Delegates.Itemize;
using Delegates.Confirm;
using Delegates.GetQueryParameters;
using Delegates.Recycle;
using Delegates.Correct;
using Delegates.Constrain;
using Delegates.Replace;
using Delegates.EnumerateIds;
using Delegates.Download;
using Delegates.Hash;
using Delegates.Trace;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Uri;
using Controllers.Network;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.StrongTypeSerialization;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Cookies;
using Controllers.Validation;
using Controllers.ValidationResult;
using Controllers.Stash;
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

using Interfaces.Delegates.Itemize;

using Interfaces.Activity;
using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;
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
using GOG.Delegates.Confirm;

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

using Models.ArgsDefinitions;
using Models.ArgsTokens;
using Models.Requests;
using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;
using Models.ValidationResult;
using Models.Status;
using Models.QueryParameters;
using Models.Directories;
using Models.Filenames;
using Models.ActivityContext;
using Models.Settings;
using Models.Template;
using Models.Patterns;

using Creators.Delegates.Convert.Requests;
using Creators.Controllers;

#endregion

namespace Ghost.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region Delegates.GetDirectory

            var getEmptyDirectoryDelegate = new GetRelativeDirectoryDelegate(string.Empty);

            var getTemplatesDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.Templates]);

            var getDataDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.Data], getEmptyDirectoryDelegate);

            var getRecycleBinDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.RecycleBin], getDataDirectoryDelegate);
            var getProductImagesDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.ProductImages], getDataDirectoryDelegate);
            var getAccountProductImagesDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.AccountProductImages], getDataDirectoryDelegate);
            var getReportDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.Reports], getDataDirectoryDelegate);
            var getValidationDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.Md5], getDataDirectoryDelegate);
            var getProductFilesBaseDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.ProductFiles], getDataDirectoryDelegate);
            var getScreenshotsDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.Screenshots], getDataDirectoryDelegate);

            var getProductFilesDirectoryDelegate = new GetUriDirectoryDelegate(getProductFilesBaseDirectoryDelegate);

            #endregion

            #region Delegates.GetFilename

            var getJsonFilenameDelegate = new GetJsonFilenameDelegate();
            var getBinFilenameDelegate = new GetBinFilenameDelegate();

            var getArgsDefinitionsFilenameDelegate = new GetFixedFilenameDelegate("definitions", getJsonFilenameDelegate);

            var getStoredHashesFilenameDelegate = new GetFixedFilenameDelegate("hashes", getBinFilenameDelegate);

            var getAppTemplateFilenameDelegate = new GetFixedFilenameDelegate("app", getJsonFilenameDelegate);
            var gerReportTemplateFilenameDelegate = new GetFixedFilenameDelegate("report", getJsonFilenameDelegate);
            var getCookiesFilenameDelegate = new GetFixedFilenameDelegate("cookies", getJsonFilenameDelegate);
            var getSettingsFilenameDelegate = new GetFixedFilenameDelegate("settings", getJsonFilenameDelegate);

            var getIndexFilenameDelegate = new GetFixedFilenameDelegate(Filenames.Base[Entity.Index], getJsonFilenameDelegate);

            var getWishlistedFilenameDelegate = new GetFixedFilenameDelegate("wishlisted", getJsonFilenameDelegate);
            var getUpdatedFilenameDelegate = new GetFixedFilenameDelegate("updated", getJsonFilenameDelegate);

            var getUriFilenameDelegate = new GetUriFilenameDelegate();
            var getReportFilenameDelegate = new GetReportFilenameDelegate();
            var getValidationFilenameDelegate = new GetValidationFilenameDelegate();

            #endregion

            #region Delegates.GetPath

            var getArgsDefinitionsPathDelegate = new GetPathDelegate(
                getEmptyDirectoryDelegate,
                getArgsDefinitionsFilenameDelegate);

            var getStoredHashesPathDelegate = new GetPathDelegate(
                getEmptyDirectoryDelegate,
                getStoredHashesFilenameDelegate);

            var getAppTemplatePathDelegate = new GetPathDelegate(
                getTemplatesDirectoryDelegate,
                getAppTemplateFilenameDelegate);

            var getReportTemplatePathDelegate = new GetPathDelegate(
                getTemplatesDirectoryDelegate,
                gerReportTemplateFilenameDelegate);

            var getCookiePathDelegate = new GetPathDelegate(
                getEmptyDirectoryDelegate,
                getCookiesFilenameDelegate);

            var getSettingsPathDelegate = new GetPathDelegate(
                getEmptyDirectoryDelegate,
                getSettingsFilenameDelegate);

            var getGameDetailsFilesPathDelegate = new GetPathDelegate(
                getProductFilesDirectoryDelegate,
                getUriFilenameDelegate);

            var getValidationPathDelegate = new GetPathDelegate(
                getValidationDirectoryDelegate,
                getValidationFilenameDelegate);

            #endregion

            var statusController = new StatusController();

            var ioOperations = new List<string>();
            var ioTraceDelegate = new IOTraceDelegate(ioOperations);

            var streamController = new StreamController();
            var fileController = new FileController();
            var directoryController = new DirectoryController();

            var storageController = new StorageController(
                streamController,
                fileController,
                ioTraceDelegate);

            var serializationController = new JSONStringController();

            var convertBytesToStringDelegate = new ConvertBytesToStringDelegate();
            var getBytesMd5HashAsyncDelegate = new GetBytesMd5HashAsyncDelegate(convertBytesToStringDelegate);
            var convertStringToBytesDelegate = new ConvertStringToBytesDelegate();
            var getStringMd5HashAsyncDelegate = new GetStringMd5HashAsyncDelegate(
                convertStringToBytesDelegate,
                getBytesMd5HashAsyncDelegate);

            var protoBufSerializedStorageController = new ProtoBufSerializedStorageController(
                fileController,
                streamController,
                statusController,
                ioTraceDelegate);

            #region Controllers.Stash

            var storedHashesStashController = new StashController<Dictionary<string, string>>(
                getStoredHashesPathDelegate,
                protoBufSerializedStorageController,
                statusController);

            #endregion

            var precomputedHashController = new StoredHashController(storedHashesStashController);

            var serializedStorageController = new SerializedStorageController(
                precomputedHashController,
                storageController,
                getStringMd5HashAsyncDelegate,
                serializationController,
                statusController);

            var argsDefinitionStashController = new StashController<ArgsDefinition>(
                getArgsDefinitionsPathDelegate,
                serializedStorageController,
                statusController);                

            #region User editable files stashControllers

            // Settings.json 

            var settingsStashController = new StashController<Settings>(
                getSettingsPathDelegate,
                serializedStorageController,
                statusController);

            // templates/app.json

            var appTemplateStashController = new StashController<List<Template>>(
                getAppTemplatePathDelegate,
                serializedStorageController,
                statusController);

            // templates/report.json

            var reportTemplateStashController = new StashController<List<Template>>(
                getReportTemplatePathDelegate,
                serializedStorageController,
                statusController);

            // cookies.json - this is required to be editable to allow user paste browser cookies

            var cookieStashController = new StashController<Dictionary<string, string>>(
                getCookiePathDelegate,
                serializedStorageController,
                statusController);

            #endregion

            var consoleController = new ConsoleController();
            var formatTextToFitConsoleWindowDelegate = new FormatTextToFitConsoleWindowDelegate(consoleController);

            var consoleInputOutputController = new ConsoleInputOutputController(
                formatTextToFitConsoleWindowDelegate,
                consoleController);

            var formatBytesDelegate = new FormatBytesDelegate();
            var formatSecondsDelegate = new FormatSecondsDelegate();

            var collectionController = new CollectionController();

            var itemizeStatusChildrenDelegate = new ItemizeStatusChildrenDelegate();

            var statusTreeToEnumerableController = new ConvertTreeToEnumerableDelegate<IStatus>(itemizeStatusChildrenDelegate);

            var applicationStatus = new Status { Title = "This ghost is a kind one." };

            var appTemplateController = new TemplateController(
                "status",
                appTemplateStashController,
                collectionController);


            var reportTemplateController = new TemplateController(
                "status",
                reportTemplateStashController,
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

            // TODO: Implement a better way
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

            var cookieSerializationController = new CookieSerializationController();

            var cookiesController = new CookiesController(
                cookieStashController,
                cookieSerializationController,
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

            var itemizeLoginIdDelegate = new ItemizeAttributeValuesDelegate(
                AttributeValuesPatterns.LoginId);
            var itemizeLoginUsernameDelegate = new ItemizeAttributeValuesDelegate(
                AttributeValuesPatterns.LoginUsername);
            var itemizeLoginTokenDelegate = new ItemizeAttributeValuesDelegate(
                AttributeValuesPatterns.LoginToken);
            var itemizeSecondStepAuthenticationTokenDelegate = new ItemizeAttributeValuesDelegate(
                AttributeValuesPatterns.SecondStepAuthenticationToken);

            var itemizeGOGDataDelegate = new ItemizeGOGDataDelegate();
            var itemizeScreenshotsDelegate = new ItemizeScreenshotsDelegate();

            var formatImagesUriDelegate = new FormatImagesUriDelegate();
            var formatScreenshotsUriDelegate = new FormatScreenshotsUriDelegate();

            var recycleDelegate = new RecycleDelegate(
                getRecycleBinDirectoryDelegate,
                fileController,
                directoryController);

            #region Correct settings 

            var correctSettingsCollectionsAsyncDelegate = new CorrectSettingsCollectionsAsyncDelegate(statusController);
            var correctSettingsDownloadsLanguagesAsyncDelegate = new CorrectSettingsDownloadsLanguagesAsyncDelegate(languageController);
            var correctSettingsDownloadsOperatingSystemsAsyncDelegate = new CorrectSettingsDownloadsOperatingSystemsAsyncDelegate();
            var correctSettingsDirectoriesAsyncDelegate = new CorrectSettingsDirectoriesAsyncDelegate();

            var correctSettingsActivity = new CorrectSettingsActivity(
                settingsStashController,
                statusController,
                correctSettingsCollectionsAsyncDelegate,
                correctSettingsDownloadsLanguagesAsyncDelegate,
                correctSettingsDownloadsOperatingSystemsAsyncDelegate,
                correctSettingsDirectoriesAsyncDelegate);

            #endregion

            #region Data Controllers

            var dataControllerFactory = new DataControllerFactory(
                protoBufSerializedStorageController,
                precomputedHashController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController);

            // TODO: Remove the stub
            Interfaces.Controllers.Index.IIndexController<long> wishlistedIndexController = null;
            //  dataControllerFactory.CreateIndexController(
            //     Entity.Wishlist, 
            //     getWishlistedFilenameDelegate);

            // TODO: Remove the stub
            Interfaces.Controllers.Index.IIndexController<long> updatedIndexController = null;
            // dataControllerFactory.CreateIndexController(
            //     Entity.Updated, 
            //     getUpdatedFilenameDelegate);

            var activityRecordsController = dataControllerFactory.CreateStringRecordsController(Entity.Activity);

            var productsDataController = dataControllerFactory.CreateDataControllerEx<Product>(Entity.Products);
            var accountProductsDataController = dataControllerFactory.CreateDataControllerEx<AccountProduct>(Entity.AccountProducts);
            var gameDetailsDataController = dataControllerFactory.CreateDataControllerEx<GameDetails>(Entity.GameDetails);
            var gameProductDataDataController = dataControllerFactory.CreateDataControllerEx<GameProductData>(Entity.GameProductData);
            var apiProductsDataController = dataControllerFactory.CreateDataControllerEx<ApiProduct>(Entity.ApiProducts);
            var productScreenshotsDataController = dataControllerFactory.CreateDataControllerEx<ProductScreenshots>(Entity.ProductScreenshots);
            var productDownloadsDataController = dataControllerFactory.CreateDataControllerEx<ProductDownloads>(Entity.ProductDownloads);
            var productRoutesDataController = dataControllerFactory.CreateDataControllerEx<ProductRoutes>(Entity.ProductRoutes);
            var validationResultsDataController = dataControllerFactory.CreateDataControllerEx<ValidationResult>(Entity.ValidationResults);

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

            #region Activity Controllers

            #region Authorize

            var correctUsernamePasswordAsyncDelegate = new CorrectUsernamePasswordAsyncDelegate(
                consoleInputOutputController);
            var correctSecurityCodeAsyncDelegate = new CorrectSecurityCodeAsyncDelegate(
                consoleInputOutputController);

            var attributeValuesItemizeDelegates = new Dictionary<string, IItemizeDelegate<string, string>>
            {
                { QueryParameters.LoginId,
                    itemizeLoginIdDelegate },
                { QueryParameters.LoginUsername,
                    itemizeLoginUsernameDelegate },
                { QueryParameters.LoginToken,
                    itemizeLoginTokenDelegate },
                { QueryParameters.SecondStepAuthenticationToken,
                    itemizeSecondStepAuthenticationTokenDelegate }
            };

            var authorizationController = new AuthorizationController(
                correctUsernamePasswordAsyncDelegate,
                correctSecurityCodeAsyncDelegate,
                uriController,
                networkController,
                serializationController,
                attributeValuesItemizeDelegates,
                statusController);

            var authorizeActivity = new AuthorizeActivity(
                settingsStashController,
                authorizationController,
                statusController);

            #endregion

            #region Update.PageResults

            var getProductUpdateUriByContextDelegate = new GetProductUpdateUriByContextDelegate();
            var getQueryParametersForProductContextDelegate = new GetQueryParametersForProductContextDelegate();

            var getProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<ProductsPageResult>(
                Entity.Products,
                getProductUpdateUriByContextDelegate,
                getQueryParametersForProductContextDelegate,
                requestPageAsyncDelegate,
                getStringMd5HashAsyncDelegate,
                precomputedHashController,
                serializationController,
                statusController);

            var itemizeProductsPageResultProductsDelegate = new ItemizeProductsPageResultProductsDelegate();

            var productsUpdateActivity = new PageResultUpdateActivity<ProductsPageResult, Product>(
                    (Activity.UpdateData, Entity.Products),
                    activityContextController,
                    getProductsPageResultsAsyncDelegate,
                    itemizeProductsPageResultProductsDelegate,
                    productsDataController,
                    activityRecordsController,
                    statusController);

            var getAccountProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<AccountProductsPageResult>(
                Entity.AccountProducts,
                getProductUpdateUriByContextDelegate,
                getQueryParametersForProductContextDelegate,
                requestPageAsyncDelegate,
                getStringMd5HashAsyncDelegate,
                precomputedHashController,
                serializationController,
                statusController);

            var itemizeAccountProductsPageResultProductsDelegate = new ItemizeAccountProductsPageResultProductsDelegate();

            var accountProductsUpdateActivity = new PageResultUpdateActivity<AccountProductsPageResult, AccountProduct>(
                    (Activity.UpdateData, Entity.AccountProducts),
                    activityContextController,
                    getAccountProductsPageResultsAsyncDelegate,
                    itemizeAccountProductsPageResultProductsDelegate,
                    accountProductsDataController,
                    activityRecordsController,
                    statusController);

            var confirmAccountProductUpdatedDelegate = new ConfirmAccountProductUpdatedDelegate();

            var updatedUpdateActivity = new UpdatedUpdateActivity(
                activityContextController,
                accountProductsDataController,
                confirmAccountProductUpdatedDelegate,
                updatedIndexController,
                statusController);

            #endregion

            #region Update.Wishlisted

            var getDeserializedPageResultAsyncDelegate = new GetDeserializedGOGDataAsyncDelegate<ProductsPageResult>(networkController,
                itemizeGOGDataDelegate,
                serializationController);

            var wishlistedUpdateActivity = new WishlistedUpdateActivity(
                getDeserializedPageResultAsyncDelegate,
                wishlistedIndexController,
                statusController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var getDeserializedGOGDataAsyncDelegate = new GetDeserializedGOGDataAsyncDelegate<GOGData>(networkController,
                itemizeGOGDataDelegate,
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
                gameProductDataDataController);

            var itemizeAllUserRequestedIdsOrDefaultAsyncDelegate = new ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
                itemizeAllUserRequestedIdsAsyncDelegate,
                itemizeAllGameProductDataGapsAsyncDelegatepsDelegate,
                updatedIndexController);

            var gameProductDataUpdateActivity = new MasterDetailProductUpdateActivity<Product, GameProductData>(
                Entity.GameProductData,
                getProductUpdateUriByContextDelegate,
                itemizeAllUserRequestedIdsOrDefaultAsyncDelegate,
                productsDataController,
                gameProductDataDataController,
                updatedIndexController,
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
                updatedIndexController);

            var apiProductUpdateActivity = new MasterDetailProductUpdateActivity<Product, ApiProduct>(
                Entity.ApiProducts,
                getProductUpdateUriByContextDelegate,
                itemizeAllUserRequestedOrApiProductGapsAndUpdatedDelegate,
                productsDataController,
                apiProductsDataController,
                updatedIndexController,
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
                updatedIndexController);

            var gameDetailsUpdateActivity = new MasterDetailProductUpdateActivity<AccountProduct, GameDetails>(
                Entity.GameDetails,
                getProductUpdateUriByContextDelegate,
                itemizeAllUserRequestedOrDefaultAsyncDelegate,
                accountProductsDataController,
                gameDetailsDataController,
                updatedIndexController,
                getDeserializedGameDetailsAsyncDelegate,
                getAccountProductUpdateIdentityDelegate,
                statusController,
                fillGameDetailsGapsDelegate);

            #endregion

            #region Update.Screenshots

            var updateScreenshotsAsyncDelegate = new UpdateScreenshotsAsyncDelegate(
                getProductUpdateUriByContextDelegate,
                productScreenshotsDataController,
                networkController,
                itemizeScreenshotsDelegate,
                statusController);

            var updateScreenshotsActivity = new UpdateScreenshotsActivity(
                productsDataController,
                productScreenshotsDataController,
                updateScreenshotsAsyncDelegate,
                statusController);

            #endregion

            // dependencies for download controllers

            var getProductImageUriDelegate = new GetProductImageUriDelegate();
            var getAccountProductImageUriDelegate = new GetAccountProductImageUriDelegate();

            var itemizeAllUserRequestedIdsOrUpdatedAsyncDelegate = new ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
                itemizeAllUserRequestedIdsAsyncDelegate,
                updatedIndexController);

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
                productScreenshotsDataController,
                formatScreenshotsUriDelegate,
                getScreenshotsDirectoryDelegate,
                fileController,
                statusController);

            var routingController = new RoutingController(
                productRoutesDataController,
                statusController);

            var itemizeGameDetailsManualUrlsAsyncDelegate = new ItemizeGameDetailsManualUrlsAsyncDelegate(
                settingsStashController,
                gameDetailsDataController);

            var itemizeGameDetailsDirectoriesAsyncDelegate = new ItemizeGameDetailsDirectoriesAsyncDelegate(
                itemizeGameDetailsManualUrlsAsyncDelegate,
                getProductFilesDirectoryDelegate);

            var itemizeGameDetailsFilesAsyncDelegate = new ItemizeGameDetailsFilesAsyncDelegate(
                itemizeGameDetailsManualUrlsAsyncDelegate,
                routingController,
                getGameDetailsFilesPathDelegate,
                statusController);

            // product files are driven through gameDetails manual urls
            // so this sources enumerates all manual urls for all updated game details
            var getManualUrlDownloadSourcesAsyncDelegate = new GetManualUrlDownloadSourcesAsyncDelegate(
                updatedIndexController,
                gameDetailsDataController,
                itemizeGameDetailsManualUrlsAsyncDelegate,
                statusController);

            // schedule download controllers

            var updateProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Entity.ProductImages,
                getProductsImagesDownloadSourcesAsyncDelegate,
                getProductImagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateAccountProductsImagesDownloadsActivity = new UpdateDownloadsActivity(
                Entity.AccountProductImages,
                getAccountProductsImagesDownloadSourcesAsyncDelegate,
                getAccountProductImagesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateScreenshotsDownloadsActivity = new UpdateDownloadsActivity(
                Entity.Screenshots,
                getScreenshotsDownloadSourcesAsyncDelegate,
                getScreenshotsDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController);

            var updateProductFilesDownloadsActivity = new UpdateDownloadsActivity(
                Entity.ProductFiles,
                getManualUrlDownloadSourcesAsyncDelegate,
                getProductFilesDirectoryDelegate,
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
                getValidationPathDelegate);

            var downloadValidationFileAsyncDelegate = new DownloadValidationFileAsyncDelegate(
                formatUriRemoveSessionDelegate,
                confirmValidationExpectedDelegate,
                formatValidationFileDelegate,
                getValidationDirectoryDelegate,
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
                Entity.ProductImages,
                productDownloadsDataController,
                downloadProductImageAsyncDelegate,
                statusController);

            var accountProductsImagesDownloadActivity = new DownloadFilesActivity(
                Entity.AccountProductImages,
                productDownloadsDataController,
                downloadProductImageAsyncDelegate,
                statusController);

            var screenshotsDownloadActivity = new DownloadFilesActivity(
                Entity.Screenshots,
                productDownloadsDataController,
                downloadProductImageAsyncDelegate,
                statusController);

            var productFilesDownloadActivity = new DownloadFilesActivity(
                Entity.ProductFiles,
                productDownloadsDataController,
                downloadManualUrlFileAsyncDelegate,
                statusController);

            // validation controllers

            var validationResultController = new ValidationResultController();

            var fileMd5Controller = new GetFileMd5HashAsyncDelegate(
                storageController,
                getStringMd5HashAsyncDelegate);

            var dataFileValidateDelegate = new DataFileValidateDelegate(
                fileMd5Controller,
                statusController);

            var productFileValidationController = new FileValidationController(
                confirmValidationExpectedDelegate,
                fileController,
                streamController,
                getBytesMd5HashAsyncDelegate,
                validationResultController,
                statusController);

            var validateProductFilesActivity = new ValidateProductFilesActivity(
                getProductFilesDirectoryDelegate,
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
                getProductFilesBaseDirectoryDelegate,
                directoryController,
                statusController);

            var itemizeDirectoryFilesDelegate = new ItemizeDirectoryFilesDelegate(directoryController);

            var directoryCleanupActivity = new CleanupActivity(
                Entity.Directories,
                itemizeAllGameDetailsDirectoriesAsyncDelegate, // expected items (directories for gameDetails)
                itemizeAllProductFilesDirectoriesAsyncDelegate, // actual items (directories in productFiles)
                itemizeDirectoryFilesDelegate, // detailed items (files in directory)
                formatValidationFileDelegate, // supplementary items (validation files)
                recycleDelegate,
                directoryController,
                statusController);

            var itemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate =
                new ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate(
                    updatedIndexController,
                    gameDetailsDataController,
                    itemizeGameDetailsFilesAsyncDelegate,
                    statusController);

            var itemizeAllUpdatedProductFilesAsyncDelegate =
                new ItemizeAllUpdatedProductFilesAsyncDelegate(
                    updatedIndexController,
                    gameDetailsDataController,
                    itemizeGameDetailsDirectoriesAsyncDelegate,
                    directoryController,
                    statusController);

            var itemizePassthroughDelegate = new ItemizePassthroughDelegate();

            var fileCleanupActivity = new CleanupActivity(
                Entity.Files,
                itemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate, // expected items (files for updated gameDetails)
                itemizeAllUpdatedProductFilesAsyncDelegate, // actual items (updated product files)
                itemizePassthroughDelegate, // detailed items (passthrough)
                formatValidationFileDelegate, // supplementary items (validation files)
                recycleDelegate,
                directoryController,
                statusController);

            var cleanupUpdatedActivity = new CleanupUpdatedActivity(
                updatedIndexController,
                statusController);

            #endregion

            #region Help

            var helpActivity = new HelpActivity(
                activityContextController,
                statusController);

            #endregion

            #region Report Task Status 

            var reportFilePresentationController = new FilePresentationController(
                getReportDirectoryDelegate,
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
                updatedIndexController,
                accountProductsDataController,
                statusController);

            #endregion

            #endregion

            #region ArgsDefinition and args

            var argsDefinitions = await argsDefinitionStashController.GetDataAsync(applicationStatus);

            if (args == null ||
                args.Length == 0)
                args = argsDefinitions.DefaultArgs.Split(" ");

            var convertArgsToRequestsDelegateCreator = 
                new ConvertArgsToRequestsDelegateCreator(
                    argsDefinitions,
                    collectionController);

            var convertArgsToRequestsDelegate =
                convertArgsToRequestsDelegateCreator.CreateDelegate();

            var requests = convertArgsToRequestsDelegate.Convert(args);                                

            #endregion

            #region Activity Context To Activity Controllers Mapping

            var activityContextToActivityControllerMap = new Dictionary<(Activity, Entity), IActivity>
            {
                { (Activity.Correct, Entity.Settings), correctSettingsActivity },
                { (Activity.Authorize, Entity.None), authorizeActivity },
                { (Activity.UpdateData, Entity.Products), productsUpdateActivity },
                { (Activity.UpdateData, Entity.AccountProducts), accountProductsUpdateActivity },
                { (Activity.UpdateData, Entity.Updated), updatedUpdateActivity },
                { (Activity.UpdateData, Entity.Wishlist), wishlistedUpdateActivity },
                { (Activity.UpdateData, Entity.GameProductData), gameProductDataUpdateActivity },
                { (Activity.UpdateData, Entity.ApiProducts), apiProductUpdateActivity },
                { (Activity.UpdateData, Entity.GameDetails), gameDetailsUpdateActivity },
                { (Activity.UpdateData, Entity.Screenshots), updateScreenshotsActivity },
                { (Activity.UpdateDownloads, Entity.ProductImages), updateProductsImagesDownloadsActivity },
                { (Activity.UpdateDownloads, Entity.AccountProductImages), updateAccountProductsImagesDownloadsActivity },
                { (Activity.UpdateDownloads, Entity.Screenshots), updateScreenshotsDownloadsActivity },
                { (Activity.UpdateDownloads, Entity.ProductFiles), updateProductFilesDownloadsActivity },
                { (Activity.Download, Entity.ProductImages), productsImagesDownloadActivity },
                { (Activity.Download, Entity.AccountProductImages), accountProductsImagesDownloadActivity },
                { (Activity.Download, Entity.Screenshots), screenshotsDownloadActivity },
                { (Activity.Download, Entity.ProductFiles), productFilesDownloadActivity },
                { (Activity.Validate, Entity.ProductFiles), validateProductFilesActivity },
                { (Activity.Validate, Entity.Data), validateDataActivity },
                { (Activity.Repair, Entity.ProductFiles), repairActivity },
                { (Activity.Cleanup, Entity.Directories), directoryCleanupActivity },
                { (Activity.Cleanup, Entity.Files), fileCleanupActivity },
                { (Activity.Cleanup, Entity.Updated), cleanupUpdatedActivity },
                { (Activity.Report, Entity.None), reportActivity },
                { (Activity.List, Entity.Updated), listUpdatedActivity },
                { (Activity.Help, Entity.None), helpActivity }
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

                    var errorMessages = new List<string>();
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
                await consoleInputOutputController.OutputContinuousAsync(applicationStatus, string.Empty, "All tasks are complete. Press ENTER to exit...");
            }

            // output IO Trace
            await storageController.PushAsync("iotrace.txt", string.Join("\n", ioOperations));

            consoleController.ReadLine();
        }
    }
}
