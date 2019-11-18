#region Using

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Delegates.Convert;
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
using Delegates.Convert.Hashes;
using Delegates.Convert.Bytes;
using Delegates.Convert.Collections;
using Delegates.Convert.Requests;
using Delegates.Convert.Collections.Status;
using Delegates.Confirm.ArgsTokens;
using Delegates.GetDirectory.Root;
using Delegates.GetDirectory.Data;
using Delegates.GetFilename.ArgsDefinitions;
using Delegates.GetFilename.Binary;
using Delegates.GetFilename.Json;
using Delegates.GetPath.ArgsDefinitions;
using Delegates.GetPath.Binary;
using Delegates.GetPath.Json;
using Delegates.GetViewModel;
using Delegates.Itemize.Attributes;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Uri;
using Controllers.Network;
using Controllers.Language;
using Controllers.Serialization.JSON;
using Controllers.StrongTypeSerialization.Cookies;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Cookies;
using Controllers.Validation;
using Controllers.ValidationResult;
using Controllers.Stash;
using Controllers.Stash.Hashes;
using Controllers.Stash.ArgsDefinitions;
using Controllers.Stash.Templates;
using Controllers.Stash.Cookies;
using Controllers.SerializedStorage.ProtoBuf;
using Controllers.SerializedStorage.JSON;
using Controllers.Presentation;
using Controllers.Routing;
using Controllers.Status;
using Controllers.Hashes;
using Controllers.Template;
using Controllers.Template.App;
using Controllers.Template.Report;
// using Controllers.ViewModel;
using Controllers.ViewUpdates;
using Controllers.InputOutput;
using Controllers.Dependencies;
using Controllers.Records.Session;

using Interfaces.Delegates.Itemize;

using Interfaces.Activity;
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
using Models.ValidationResults;
using Models.Status;
using Models.QueryParameters;
using Models.Directories;
using Models.Filenames;
using Models.Settings;
using Models.Template;
using Models.Patterns;
using Models.Records;

using Creators.Delegates.Convert.Requests;
using Creators.Controllers;

#endregion

namespace vangogh.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dependenciesController = new DependenciesController();
            var dependenciesControllerInitializer = new DependenciesControllerInitializer(dependenciesController);

            dependenciesControllerInitializer.Initialize();

            #region Processed Dependencies

            // var getEmptyDirectoryDelegate = dependenciesController.GetInstance(
            //     typeof(GetEmptyDirectoryDelegate))
            //     as GetEmptyDirectoryDelegate;

            // var getTemplatesDirectoryDelegate = dependenciesController.GetInstance(
            //     typeof(GetTemplatesDirectoryDelegate))
            //     as GetTemplatesDirectoryDelegate;

            var getDataDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetDataDirectoryDelegate))
                as GetDataDirectoryDelegate;

            // var getRecycleBinDirectoryDelegate = dependenciesController.GetInstance(
            //     typeof(GetRecycleBinDirectoryDelegate))
            //     as GetRecycleBinDirectoryDelegate;

            var getProductImagesDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetProductImagesDirectoryDelegate))
                as GetProductImagesDirectoryDelegate;

            var getAccountProductImagesDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetAccountProductImagesDirectoryDelegate))
                as GetAccountProductImagesDirectoryDelegate;

            var getReportDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetReportDirectoryDelegate))
                as GetReportDirectoryDelegate;

            var getMd5DirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetMd5DirectoryDelegate))
                as GetMd5DirectoryDelegate;

            var getProductFilesRootDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetProductFilesRootDirectoryDelegate))
                as GetProductFilesRootDirectoryDelegate;

            var getScreenshotsDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetScreenshotsDirectoryDelegate))
                as GetScreenshotsDirectoryDelegate;

            var getProductFilesDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetProductFilesDirectoryDelegate))
                as GetProductFilesDirectoryDelegate;

            // var getJsonFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetJsonFilenameDelegate))
            //     as GetJsonFilenameDelegate;

            var getBinFilenameDelegate = dependenciesController.GetInstance(
                typeof(GetBinFilenameDelegate))
                as GetBinFilenameDelegate;

            // var getArgsDefinitionsFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetArgsDefinitionsFilenameDelegate))
            //     as GetArgsDefinitionsFilenameDelegate;

            // var getHashesFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetHashesFilenameDelegate))
            //     as GetHashesFilenameDelegate;

            // var getAppTemplateFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetAppTemplateFilenameDelegate))
            //     as GetAppTemplateFilenameDelegate;

            // var getReportTemplateFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetReportTemplateFilenameDelegate))
            //     as GetReportTemplateFilenameDelegate;                

            // var getCookiesFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetCookiesFilenameDelegate))
            //     as GetCookiesFilenameDelegate;  

            // var getIndexFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetIndexFilenameDelegate))
            //     as GetIndexFilenameDelegate;

            // var getWishlistedFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetWishlistedFilenameDelegate))
            //     as GetWishlistedFilenameDelegate;

            // var getUpdatedFilenameDelegate = dependenciesController.GetInstance(
            //     typeof(GetUpdatedFilenameDelegate))
            //     as GetUpdatedFilenameDelegate;

            var getUriFilenameDelegate = dependenciesController.GetInstance(
                typeof(GetUriFilenameDelegate))
                as GetUriFilenameDelegate;

            var getReportFilenameDelegate = dependenciesController.GetInstance(
                typeof(GetReportFilenameDelegate))
                as GetReportFilenameDelegate;

            var getValidationFilenameDelegate = dependenciesController.GetInstance(
                typeof(GetValidationFilenameDelegate))
                as GetValidationFilenameDelegate;

            // var getArgsDefinitionsPathDelegate = dependenciesController.GetInstance(
            //     typeof(GetArgsDefinitionsPathDelegate))
            //     as GetArgsDefinitionsPathDelegate;

            // var getHashesPathDelegate = dependenciesController.GetInstance(
            //     typeof(GetHashesPathDelegate))
            //     as GetHashesPathDelegate;

            // var getAppTemplatePathDelegate = dependenciesController.GetInstance(
            //     typeof(GetAppTemplatePathDelegate))
            //     as GetAppTemplatePathDelegate;

            // var getReportTemplatePathDelegate = dependenciesController.GetInstance(
            //     typeof(GetReportTemplatePathDelegate))
            //     as GetReportTemplatePathDelegate;

            // var getCookiePathDelegate = dependenciesController.GetInstance(
            //     typeof(GetCookiesPathDelegate))
            //     as GetCookiesPathDelegate;

            var getGameDetailsFilesPathDelegate = dependenciesController.GetInstance(
                typeof(GetGameDetailsFilesPathDelegate))
                as GetGameDetailsFilesPathDelegate;

            var getValidationPathDelegate = dependenciesController.GetInstance(
                typeof(GetValidationPathDelegate))
                as GetValidationPathDelegate;

            var statusController = dependenciesController.GetInstance(
                typeof(StatusController))
                as StatusController;

            var streamController = dependenciesController.GetInstance(
                typeof(StreamController))
                as StreamController;

            var fileController = dependenciesController.GetInstance(
                typeof(FileController))
                as FileController;

            var directoryController = dependenciesController.GetInstance(
                typeof(DirectoryController))
                as DirectoryController;

            var storageController = dependenciesController.GetInstance(
                typeof(StorageController))
                as StorageController;

            var jsonSerializationController = dependenciesController.GetInstance(
                typeof(JSONSerializationController))
                as JSONSerializationController;

            // var convertBytesToStringDelegate = dependenciesController.GetInstance(
            //     typeof(ConvertBytesToStringDelegate))
            //     as ConvertBytesToStringDelegate;

            var convertBytesToMd5HashDelegate = dependenciesController.GetInstance(
                typeof(ConvertBytesToMd5HashDelegate))
                as ConvertBytesToMd5HashDelegate;

            // var convertStringToBytesDelegate = dependenciesController.GetInstance(
            //     typeof(ConvertStringToBytesDelegate))
            //     as ConvertStringToBytesDelegate;

            var convertStringToMd5HashDelegate = dependenciesController.GetInstance(
                typeof(ConvertStringToMd5HashDelegate))
                as ConvertStringToMd5HashDelegate;

            var protoBufSerializedStorageController = dependenciesController.GetInstance(
                typeof(ProtoBufSerializedStorageController))
                as ProtoBufSerializedStorageController;

            // var hashesStashController = dependenciesController.GetInstance(
            //     typeof(HashesStashController))
            //     as HashesStashController;

            // TODO: Review if still needed/desired to have this
            var hashesController = dependenciesController.GetInstance(
                typeof(HashesController))
                as HashesController;

            // var jsonSerializedStorageController = dependenciesController.GetInstance(
            //     typeof(JSONSerializedStorageController))
            //     as JSONSerializedStorageController;

            var argsDefinitionStashController = dependenciesController.GetInstance(
                typeof(ArgsDefinitionsStashController))
                as ArgsDefinitionsStashController;

            // var appTemplateStashController = dependenciesController.GetInstance(
            //     typeof(AppTemplateStashController))
            //     as AppTemplateStashController;

            // var reportTemplateStashController = dependenciesController.GetInstance(
            //     typeof(ReportTemplateStashController))
            //     as ReportTemplateStashController;

            // var cookiesStashController = dependenciesController.GetInstance(
            //     typeof(CookiesStashController))
            //     as CookiesStashController;

            // var consoleController = dependenciesController.GetInstance(
            //     typeof(ConsoleController))
            //     as ConsoleController;

            // var formatTextToFitConsoleWindowDelegate = dependenciesController.GetInstance(
            //     typeof(FormatTextToFitConsoleWindowDelegate))
            //     as FormatTextToFitConsoleWindowDelegate;

            var consoleInputOutputController = dependenciesController.GetInstance(
                typeof(ConsoleInputOutputController))
                as ConsoleInputOutputController;

            // var formatBytesDelegate = dependenciesController.GetInstance(
            //     typeof(FormatBytesDelegate))
            //     as FormatBytesDelegate;

            // var formatSecondsDelegate = dependenciesController.GetInstance(
            //     typeof(FormatSecondsDelegate))
            //     as FormatSecondsDelegate;

            var collectionController = dependenciesController.GetInstance(
                typeof(CollectionController))
                as CollectionController;

            // var itemizeStatusChildrenDelegate = dependenciesController.GetInstance(
            //     typeof(ItemizeStatusChildrenDelegate))
            //     as ItemizeStatusChildrenDelegate;

            // var convertStatusTreeToEnumerableDelegate = dependenciesController.GetInstance(
            //     typeof(ConvertStatusTreeToEnumerableDelegate))
            //     as ConvertStatusTreeToEnumerableDelegate;

            var applicationStatus = dependenciesController.GetInstance(
                typeof(Status))
                as Status;

            // var appTemplateController = dependenciesController.GetInstance(
            //     typeof(AppTemplateController))
            //     as AppTemplateController;

            // var reportTemplateController = dependenciesController.GetInstance(
            //     typeof(ReportTemplateController))
            //     as ReportTemplateController;

            // var formatRemainingTimeAtSpeedDelegate = dependenciesController.GetInstance(
            //     typeof(FormatRemainingTimeAtSpeedDelegate))
            //     as FormatRemainingTimeAtSpeedDelegate;

            // var getStatusAppViewModelDelegate = dependenciesController.GetInstance(
            //     typeof(GetStatusAppViewModelDelegate))
            //     as GetStatusAppViewModelDelegate;

            // var statusReportViewModelDelegate = dependenciesController.GetInstance(
            //     typeof(GetStatusReportViewModelDelegate))
            //     as GetStatusReportViewModelDelegate;

            var getStatusViewUpdateDelegate = dependenciesController.GetInstance(
                typeof(GetStatusViewUpdateDelegate))
                as GetStatusViewUpdateDelegate;

            // var consoleNotifyStatusViewUpdateController = dependenciesController.GetInstance(
            //     typeof(NotifyStatusViewUpdateController))
            //     as NotifyStatusViewUpdateController;

            // TODO: Implement a better way
            // add notification handler to drive console view updates
            // statusController.NotifyStatusChangedAsync += consoleNotifyStatusViewUpdateController.NotifyViewUpdateOutputOnRefreshAsync;

            // var constrainExecutionAsyncDelegate = dependenciesController.GetInstance(
            //     typeof(ConstrainExecutionAsyncDelegate))
            //     as ConstrainExecutionAsyncDelegate;

            // var itemizeAllRateConstrainedUrisDelegate = dependenciesController.GetInstance(
            //     typeof(ItemizeAllRateConstrainedUrisDelegate))
            //     as ItemizeAllRateConstrainedUrisDelegate;

            // var constrainRequestRateAsyncDelegate = dependenciesController.GetInstance(
            //     typeof(ConstrainRequestRateAsyncDelegate))
            //     as ConstrainRequestRateAsyncDelegate;;

            var uriController = dependenciesController.GetInstance(
                typeof(UriController))
                as UriController;

            // var cookiesSerializationController = dependenciesController.GetInstance(
            //     typeof(CookiesSerializationController))
            //     as CookiesSerializationController;

            // var cookiesController = dependenciesController.GetInstance(
            //     typeof(CookiesController))
            //     as CookiesController;

            var networkController = dependenciesController.GetInstance(
                typeof(NetworkController))
                as NetworkController;

            var downloadFromResponseAsyncDelegate = dependenciesController.GetInstance(
                typeof(DownloadFromResponseAsyncDelegate))
                as DownloadFromResponseAsyncDelegate;

            var downloadFromUriAsyncDelegate = dependenciesController.GetInstance(
                typeof(DownloadFromUriAsyncDelegate))
                as DownloadFromUriAsyncDelegate;

            var requestPageAsyncDelegate = dependenciesController.GetInstance(
                typeof(RequestPageAsyncDelegate))
                as RequestPageAsyncDelegate;

            var languageController = dependenciesController.GetInstance(
                typeof(LanguageController))
                as LanguageController;

            var itemizeLoginIdDelegate = dependenciesController.GetInstance(
                typeof(ItemizeLoginIdAttributeValuesDelegate))
                as ItemizeLoginIdAttributeValuesDelegate;

            var itemizeLoginUsernameDelegate = dependenciesController.GetInstance(
                typeof(ItemizeLoginUsernameAttributeValuesDelegate))
                as ItemizeLoginUsernameAttributeValuesDelegate;

            var itemizeLoginTokenDelegate = dependenciesController.GetInstance(
                typeof(ItemizeLoginTokenAttributeValuesDelegate))
                as ItemizeLoginTokenAttributeValuesDelegate;

            var itemizeSecondStepAuthenticationTokenDelegate = dependenciesController.GetInstance(
                typeof(ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate))
                as ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate;

            var itemizeGOGDataDelegate = dependenciesController.GetInstance(
                typeof(ItemizeGOGDataDelegate))
                as ItemizeGOGDataDelegate;

            var itemizeScreenshotsDelegate = dependenciesController.GetInstance(
                typeof(ItemizeScreenshotsDelegate))
                as ItemizeScreenshotsDelegate; ;

            var formatImagesUriDelegate = dependenciesController.GetInstance(
                typeof(FormatImagesUriDelegate))
                as FormatImagesUriDelegate;

            var formatScreenshotsUriDelegate = dependenciesController.GetInstance(
                typeof(FormatScreenshotsUriDelegate))
                as FormatScreenshotsUriDelegate;

            var recycleDelegate = dependenciesController.GetInstance(
                typeof(RecycleDelegate))
                as RecycleDelegate;

            #endregion

            #region Data Controllers

            var dataControllerFactory = new DataControllerFactory();

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

            var sessionRecordsController = dependenciesController.GetInstance(
                typeof(SessionRecordsController))
                as SessionRecordsController;

            var convertProductRecordToIndexDelegate = dependenciesController.GetInstance(
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>))
                as ConvertProductCoreToIndexDelegate<ProductRecords>;

            var getRecordsDirectoryDelegate = dependenciesController.GetInstance(
                typeof(GetRecordsDirectoryDelegate))
                as GetRecordsDirectoryDelegate;

            var productsDataController = dataControllerFactory.CreateDataControllerEx<Product>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var accountProductsDataController = dataControllerFactory.CreateDataControllerEx<AccountProduct>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var gameDetailsDataController = dataControllerFactory.CreateDataControllerEx<GameDetails>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var gameProductDataDataController = dataControllerFactory.CreateDataControllerEx<GameProductData>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var apiProductsDataController = dataControllerFactory.CreateDataControllerEx<ApiProduct>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var productScreenshotsDataController = dataControllerFactory.CreateDataControllerEx<ProductScreenshots>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var productDownloadsDataController = dataControllerFactory.CreateDataControllerEx<ProductDownloads>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var productRoutesDataController = dataControllerFactory.CreateDataControllerEx<ProductRoutes>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);
            var validationResultsDataController = dataControllerFactory.CreateDataControllerEx<ValidationResults>(
                convertProductRecordToIndexDelegate,
                getRecordsDirectoryDelegate,                
                protoBufSerializedStorageController,
                hashesController,
                getDataDirectoryDelegate,
                getBinFilenameDelegate,
                statusController,
                dependenciesController);

            #endregion

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
                jsonSerializationController,
                attributeValuesItemizeDelegates,
                statusController);

            var authorizeActivity = new AuthorizeActivity(
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
                convertStringToMd5HashDelegate,
                hashesController,
                jsonSerializationController,
                statusController);

            var itemizeProductsPageResultProductsDelegate = new ItemizeProductsPageResultProductsDelegate();

            var productsUpdateActivity = new PageResultUpdateActivity<ProductsPageResult, Product>(
                    // (Activity.UpdateData, Entity.Products),
                    getProductsPageResultsAsyncDelegate,
                    itemizeProductsPageResultProductsDelegate,
                    productsDataController,
                    sessionRecordsController,
                    statusController);

            var getAccountProductsPageResultsAsyncDelegate = new GetPageResultsAsyncDelegate<AccountProductsPageResult>(
                Entity.AccountProducts,
                getProductUpdateUriByContextDelegate,
                getQueryParametersForProductContextDelegate,
                requestPageAsyncDelegate,
                convertStringToMd5HashDelegate,
                hashesController,
                jsonSerializationController,
                statusController);

            var itemizeAccountProductsPageResultProductsDelegate = new ItemizeAccountProductsPageResultProductsDelegate();

            var accountProductsUpdateActivity = new PageResultUpdateActivity<AccountProductsPageResult, AccountProduct>(
                    // (Activity.UpdateData, Entity.AccountProducts),
                    getAccountProductsPageResultsAsyncDelegate,
                    itemizeAccountProductsPageResultProductsDelegate,
                    accountProductsDataController,
                    sessionRecordsController,
                    statusController);

            var confirmAccountProductUpdatedDelegate = new ConfirmAccountProductUpdatedDelegate();

            var updatedUpdateActivity = new UpdatedUpdateActivity(
                accountProductsDataController,
                confirmAccountProductUpdatedDelegate,
                updatedIndexController,
                statusController);

            #endregion

            #region Update.Wishlisted

            var getDeserializedPageResultAsyncDelegate = new GetDeserializedGOGDataAsyncDelegate<ProductsPageResult>(networkController,
                itemizeGOGDataDelegate,
                jsonSerializationController);

            var wishlistedUpdateActivity = new WishlistedUpdateActivity(
                getDeserializedPageResultAsyncDelegate,
                wishlistedIndexController,
                statusController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var getDeserializedGOGDataAsyncDelegate = new GetDeserializedGOGDataAsyncDelegate<GOGData>(networkController,
                itemizeGOGDataDelegate,
                jsonSerializationController);

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
                jsonSerializationController);

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
                jsonSerializationController);

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
                jsonSerializationController,
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
                getMd5DirectoryDelegate,
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

            var convertFileToMd5HashDelegate = new ConvertFileToMd5HashDelegate(
                storageController,
                convertStringToMd5HashDelegate);

            var dataFileValidateDelegate = new DataFileValidateDelegate(
                convertFileToMd5HashDelegate,
                statusController);

            var productFileValidationController = new FileValidationController(
                confirmValidationExpectedDelegate,
                fileController,
                streamController,
                convertBytesToMd5HashDelegate,
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
                hashesController,
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
                getProductFilesRootDirectoryDelegate,
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
                    argsDefinitionStashController,
                    collectionController,
                    dependenciesController);

            var confirmLikelyTokenTypeDelegate = dependenciesController.GetInstance(
                typeof(ConfirmLikelyTokenTypeDelegate))
                as ConfirmLikelyTokenTypeDelegate;

            // var convertArgsToRequestsDelegate =
            //     convertArgsToRequestsDelegateCreator.CreateDelegate();

            //var requests = convertArgsToRequestsDelegate.Convert(args);                                

            #endregion

            #region Activity Context To Activity Controllers Mapping

            // var activityContextToActivityControllerMap = new Dictionary<(Activity, Entity), IActivity>
            // {
            //     { (Activity.Authorize, Entity.None), authorizeActivity },
            //     { (Activity.UpdateData, Entity.Products), productsUpdateActivity },
            //     { (Activity.UpdateData, Entity.AccountProducts), accountProductsUpdateActivity },
            //     { (Activity.UpdateData, Entity.Updated), updatedUpdateActivity },
            //     { (Activity.UpdateData, Entity.Wishlist), wishlistedUpdateActivity },
            //     { (Activity.UpdateData, Entity.GameProductData), gameProductDataUpdateActivity },
            //     { (Activity.UpdateData, Entity.ApiProducts), apiProductUpdateActivity },
            //     { (Activity.UpdateData, Entity.GameDetails), gameDetailsUpdateActivity },
            //     { (Activity.UpdateData, Entity.Screenshots), updateScreenshotsActivity },
            //     { (Activity.UpdateDownloads, Entity.ProductImages), updateProductsImagesDownloadsActivity },
            //     { (Activity.UpdateDownloads, Entity.AccountProductImages), updateAccountProductsImagesDownloadsActivity },
            //     { (Activity.UpdateDownloads, Entity.Screenshots), updateScreenshotsDownloadsActivity },
            //     { (Activity.UpdateDownloads, Entity.ProductFiles), updateProductFilesDownloadsActivity },
            //     { (Activity.Download, Entity.ProductImages), productsImagesDownloadActivity },
            //     { (Activity.Download, Entity.AccountProductImages), accountProductsImagesDownloadActivity },
            //     { (Activity.Download, Entity.Screenshots), screenshotsDownloadActivity },
            //     { (Activity.Download, Entity.ProductFiles), productFilesDownloadActivity },
            //     { (Activity.Validate, Entity.ProductFiles), validateProductFilesActivity },
            //     { (Activity.Validate, Entity.Data), validateDataActivity },
            //     { (Activity.Repair, Entity.ProductFiles), repairActivity },
            //     { (Activity.Cleanup, Entity.Directories), directoryCleanupActivity },
            //     { (Activity.Cleanup, Entity.Files), fileCleanupActivity },
            //     { (Activity.Cleanup, Entity.Updated), cleanupUpdatedActivity },
            //     { (Activity.Report, Entity.None), reportActivity },
            //     { (Activity.List, Entity.Updated), listUpdatedActivity },
            //     { (Activity.Help, Entity.None), helpActivity }
            // };

            #endregion

            #region Core Activities Loop

            // foreach (var activityContext in activityContextQueue)
            // {
            //     if (!activityContextToActivityControllerMap.ContainsKey(activityContext))
            //     {
            //         await statusController.WarnAsync(
            //             applicationStatus,
            //             activityContextController.ToString(activityContext) + " is not mapped to an Activity.");
            //         continue;
            //     }

            //     var activity = activityContextToActivityControllerMap[activityContext];
            //     try
            //     {
            //         await activity.ProcessActivityAsync(applicationStatus);
            //     }
            //     catch (AggregateException ex)
            //     {
            //         var itemizeInnerExceptionsDelegate = new ItemizeInnerExceptionsDelegate();
            //         var convertTreeToEnumerableDelegate = new ConvertTreeToEnumerableDelegate<Exception>(itemizeInnerExceptionsDelegate);

            //         var errorMessages = new List<string>();
            //         foreach (var innerException in convertTreeToEnumerableDelegate.Convert(ex))
            //             errorMessages.Add(innerException.Message);

            //         var combinedErrorMessages = string.Join(Models.Separators.Separators.Common.Comma, errorMessages);

            //         await statusController.FailAsync(applicationStatus, combinedErrorMessages);

            //         var failureDumpUri = "failureDump.json";
            //         await jsonSerializedStorageController.SerializePushAsync(failureDumpUri, applicationStatus, applicationStatus);

            //         await consoleInputOutputController.OutputOnRefreshAsync(
            //             "GoodOfflineGames.exe has encountered fatal error(s): " +
            //             combinedErrorMessages +
            //             $".\nPlease refer to {failureDumpUri} for further details.\n" +
            //             "Press ENTER to close the window...");

            //         consoleController.ReadLine();

            //         return;
            //     }
            // }

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

            System.Console.ReadLine();
        }
    }
}
