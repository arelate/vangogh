using Interfaces.Controllers.Dependencies;

using Controllers.Collection;
using Controllers.Stash.ArgsDefinitions;
using Controllers.Stash.Templates;
using Controllers.Stash.Cookies;
using Controllers.Stash.Records;
using Controllers.Stash.ProductTypes;
using Controllers.File;
using Controllers.Directory;
using Controllers.Storage;
using Controllers.Stream;
using Controllers.SerializedStorage.ProtoBuf;
using Controllers.SerializedStorage.JSON;
using Controllers.Serialization.JSON;
using Controllers.Status;
using Controllers.Console;
using Controllers.InputOutput;
using Controllers.Template.App;
using Controllers.Template.Report;
using Controllers.ViewUpdates;
using Controllers.Cookies;
using Controllers.StrongTypeSerialization.Cookies;
using Controllers.Network;
using Controllers.Uri;
using Controllers.Data.Records;
using Controllers.Data.ProductTypes;
using Controllers.Records.Session;
using Controllers.Records.ProductTypes;

using Delegates.Correct;
using Delegates.Itemize.Attributes;
using Delegates.GetDirectory.Root;
using Delegates.GetDirectory.ProductTypes;
using Delegates.GetFilename;
using Delegates.GetFilename.ArgsDefinitions;
using Delegates.GetFilename.Binary;
using Delegates.GetFilename.Json;
using Delegates.GetFilename.ProductTypes;
using Delegates.GetPath.ArgsDefinitions;
using Delegates.GetPath.Json;
using Delegates.GetPath.Records;
using Delegates.GetPath.ProductTypes;
using Delegates.Convert;
using Delegates.Convert.Hashes;
using Delegates.Convert.Bytes;
using Delegates.Convert.Collections.Status;
using Delegates.Convert.ProductTypes;
using Delegates.Recycle;
using Delegates.Confirm.ArgsTokens;
using Delegates.Convert.ArgsTokens;
using Delegates.Itemize;
using Delegates.GetViewModel;
using Delegates.Format.Status;
using Delegates.Format.Numbers;
using Delegates.Format.Text;
using Delegates.Format.Uri;
using Delegates.Constrain;
using Delegates.Download;
using Delegates.GetValue.Uri.ProductTypes;
using Delegates.GetValue.QueryParameters.ProductTypes;

using GOG.Delegates.Itemize;
using GOG.Delegates.RequestPage;
using GOG.Delegates.Convert.ProductTypes;
using GOG.Delegates.GetPageResults.ProductTypes;
using GOG.Delegates.Confirm;

using GOG.Controllers.Stash.ProductTypes;
using GOG.Controllers.Data.ProductTypes;
using GOG.Controllers.Authorization;

using GOG.Activities.Authorize;
using GOG.Activities.Update.ProductTypes;

using Models.Records;

namespace vangogh.Console
{
    public class DependenciesControllerInitializer
    {
        private readonly IDependenciesController dependenciesController;
        public DependenciesControllerInitializer(
            IDependenciesController dependenciesController)
        {
            this.dependenciesController = dependenciesController;
        }

        public void Initialize()
        {
            // Delegate.GetDirectory

            dependenciesController.AddDependencies<GetRecycleBinDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetProductImagesDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetAccountProductImagesDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetReportDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetMd5DirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetProductFilesRootDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetScreenshotsDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            dependenciesController.AddDependencies<GetProductFilesDirectoryDelegate>(
                typeof(GetProductFilesRootDirectoryDelegate));

            dependenciesController.AddDependencies<GetRecordsDirectoryDelegate>(
                typeof(GetDataDirectoryDelegate));

            // Delegates.GetFilename

            dependenciesController.AddDependencies<GetArgsDefinitionsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetAppTemplateFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetReportTemplateFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetCookiesFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetIndexFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetSessionRecordsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetAccountProductsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetApiProductsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetGameDetailsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetGameProductDataFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetProductDownloadsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetProductRoutesFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetProductScreenshotsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetProductsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetValidationResultsFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetWishlistedFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetUpdatedFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));                

            // Delegates.GetPath

            dependenciesController.AddDependencies<GetArgsDefinitionsPathDelegate>(
                typeof(GetEmptyDirectoryDelegate),
                typeof(GetArgsDefinitionsFilenameDelegate));

            dependenciesController.AddDependencies<GetAppTemplatePathDelegate>(
                typeof(GetTemplatesDirectoryDelegate),
                typeof(GetAppTemplateFilenameDelegate));

            dependenciesController.AddDependencies<GetReportTemplatePathDelegate>(
                typeof(GetTemplatesDirectoryDelegate),
                typeof(GetReportTemplateFilenameDelegate));

            dependenciesController.AddDependencies<GetCookiesPathDelegate>(
                typeof(GetEmptyDirectoryDelegate),
                typeof(GetCookiesFilenameDelegate));

            dependenciesController.AddDependencies<GetGameDetailsFilesPathDelegate>(
                typeof(GetProductFilesDirectoryDelegate),
                typeof(GetUriFilenameDelegate));

            dependenciesController.AddDependencies<GetValidationPathDelegate>(
                typeof(GetMd5DirectoryDelegate),
                typeof(GetValidationFilenameDelegate));

            // Delegates.GetPath.Records

            dependenciesController.AddDependencies<GetSessionRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetSessionRecordsFilenameDelegate));

            dependenciesController.AddDependencies<GetAccountProductsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetAccountProductsFilenameDelegate));

            dependenciesController.AddDependencies<GetApiProductsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetApiProductsFilenameDelegate));

            dependenciesController.AddDependencies<GetGameDetailsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetGameDetailsFilenameDelegate));

            dependenciesController.AddDependencies<GetGameProductDataRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetGameProductDataFilenameDelegate));

            dependenciesController.AddDependencies<GetProductDownloadsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetProductDownloadsFilenameDelegate));

            dependenciesController.AddDependencies<GetProductRoutesRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetProductRoutesFilenameDelegate));

            dependenciesController.AddDependencies<GetProductScreenshotsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetProductScreenshotsFilenameDelegate));

            dependenciesController.AddDependencies<GetProductsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetProductsFilenameDelegate));

            dependenciesController.AddDependencies<GetValidationResultsRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetValidationResultsFilenameDelegate));

            dependenciesController.AddDependencies<GetUpdatedRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetUpdatedFilenameDelegate));     

            dependenciesController.AddDependencies<GetWishlistedRecordsPathDelegate>(
                typeof(GetRecordsDirectoryDelegate),
                typeof(GetWishlistedFilenameDelegate));                           

            // Delegates.GetPath.ProductTypes

            dependenciesController.AddDependencies<GetAccountProductsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetAccountProductsFilenameDelegate));

            dependenciesController.AddDependencies<GetApiProductsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetApiProductsFilenameDelegate));

            dependenciesController.AddDependencies<GetGameDetailsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetGameDetailsFilenameDelegate));

            dependenciesController.AddDependencies<GetGameProductDataPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetGameProductDataFilenameDelegate));

            dependenciesController.AddDependencies<GetProductDownloadsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetProductDownloadsFilenameDelegate));

            dependenciesController.AddDependencies<GetProductRoutesPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetProductRoutesFilenameDelegate));

            dependenciesController.AddDependencies<GetProductScreenshotsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetProductScreenshotsFilenameDelegate));

            dependenciesController.AddDependencies<GetProductsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetProductsFilenameDelegate));

            dependenciesController.AddDependencies<GetValidationResultsPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetValidationResultsFilenameDelegate));

            dependenciesController.AddDependencies<GetUpdatedPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetUpdatedFilenameDelegate));

            dependenciesController.AddDependencies<GetWishlistedPathDelegate>(
                typeof(GetDataDirectoryDelegate),
                typeof(GetWishlistedFilenameDelegate));                                

            // Controllers.Storage

            dependenciesController.AddDependencies<StorageController>(
                typeof(StreamController),
                typeof(FileController));

            // Delegates.GetHash

            dependenciesController.AddDependencies<ConvertBytesToMd5HashDelegate>(
                typeof(ConvertBytesToStringDelegate));

            dependenciesController.AddDependencies<ConvertStringToMd5HashDelegate>(
                typeof(ConvertStringToBytesDelegate),
                typeof(ConvertBytesToMd5HashDelegate));

            // Controllers.SerializedStorage

            dependenciesController.AddDependencies<ProtoBufSerializedStorageController>(
                typeof(FileController),
                typeof(StreamController),
                typeof(StatusController));

            // Controllers.Stash.Records

            dependenciesController.AddDependencies<SessionRecordsStashController>(
                typeof(GetSessionRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<AccountProductsRecordsStashController>(
                typeof(GetAccountProductsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ApiProductsRecordsStashController>(
                typeof(GetApiProductsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameDetailsRecordsStashController>(
                typeof(GetGameDetailsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameProductDataRecordsStashController>(
                typeof(GetGameProductDataRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductDownloadsRecordsStashController>(
                typeof(GetProductDownloadsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductRoutesRecordsStashController>(
                typeof(GetProductRoutesRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductScreenshotsRecordsStashController>(
                typeof(GetProductScreenshotsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductsRecordsStashController>(
                typeof(GetProductsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ValidationResultsRecordsStashController>(
                typeof(GetValidationResultsRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<UpdatedRecordsStashController>(
                typeof(GetUpdatedRecordsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<WishlistedRecordsStashController>(
                typeof(GetWishlistedPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));                     

            // Controllers.SerializedStorage.JSON

            dependenciesController.AddDependencies<JSONSerializedStorageController>(
                typeof(StorageController),
                typeof(JSONSerializationController),
                typeof(StatusController));

            // Controllers.Stash.ArgsDefinition

            dependenciesController.AddDependencies<ArgsDefinitionsStashController>(
                typeof(GetArgsDefinitionsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            // Controllers.Stash.Templates

            dependenciesController.AddDependencies<AppTemplateStashController>(
                typeof(GetAppTemplatePathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ReportTemplateStashController>(
                typeof(GetReportTemplatePathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            // Controllers.Stash.Cookies

            dependenciesController.AddDependencies<CookiesStashController>(
                typeof(GetCookiesPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            // Delegates.Format.Text

            dependenciesController.AddDependencies<FormatTextToFitConsoleWindowDelegate>(
                typeof(ConsoleController));

            // Controllers.InputOutput

            dependenciesController.AddDependencies<ConsoleInputOutputController>(
                typeof(FormatTextToFitConsoleWindowDelegate),
                typeof(ConsoleController));

            // Delegates.Convert.Collections.Status

            dependenciesController.AddDependencies<ConvertStatusTreeToEnumerableDelegate>(
                typeof(ItemizeStatusChildrenDelegate));

            // Controllers.Template

            dependenciesController.AddDependencies<AppTemplateController>(
                typeof(AppTemplateStashController),
                typeof(CollectionController));

            dependenciesController.AddDependencies<ReportTemplateController>(
                typeof(ReportTemplateStashController),
                typeof(CollectionController));

            // Delegates.GetViewModel

            dependenciesController.AddDependencies<GetStatusAppViewModelDelegate>(
                typeof(FormatRemainingTimeAtSpeedDelegate),
                typeof(FormatBytesDelegate),
                typeof(FormatSecondsDelegate));

            dependenciesController.AddDependencies<GetStatusReportViewModelDelegate>(
                typeof(FormatBytesDelegate),
                typeof(FormatSecondsDelegate));

            // Controllers.ViewUpdates

            dependenciesController.AddDependencies<GetStatusViewUpdateDelegate>(
                typeof(AppTemplateController),
                typeof(GetStatusAppViewModelDelegate),
                typeof(ConvertStatusTreeToEnumerableDelegate));

            dependenciesController.AddDependencies<NotifyStatusViewUpdateController>(
                typeof(GetStatusViewUpdateDelegate),
                typeof(ConsoleInputOutputController));

            // Delegates.Constrain

            dependenciesController.AddDependencies<ConstrainExecutionAsyncDelegate>(
                typeof(StatusController),
                typeof(FormatSecondsDelegate));

            dependenciesController.AddDependencies<ConstrainRequestRateAsyncDelegate>(
                typeof(ConstrainExecutionAsyncDelegate),
                typeof(CollectionController),
                typeof(StatusController),
                typeof(ItemizeAllRateConstrainedUrisDelegate));

            // Controllers.Cookies

            dependenciesController.AddDependencies<CookiesController>(
                typeof(CookiesStashController),
                typeof(CookiesSerializationController),
                typeof(StatusController));

            // Controllers.Network

            dependenciesController.AddDependencies<NetworkController>(
                typeof(CookiesController),
                typeof(UriController),
                typeof(ConstrainRequestRateAsyncDelegate));

            // Delegates.Download

            dependenciesController.AddDependencies<DownloadFromResponseAsyncDelegate>(
                typeof(NetworkController),
                typeof(StreamController),
                typeof(FileController),
                typeof(StatusController));

            dependenciesController.AddDependencies<DownloadFromUriAsyncDelegate>(
                typeof(NetworkController),
                typeof(DownloadFromResponseAsyncDelegate),
                typeof(StatusController));

            dependenciesController.AddDependencies<RequestPageAsyncDelegate>(
                typeof(NetworkController));

            // Controllers.Data.Records

            dependenciesController.AddDependencies<SessionRecordsDataController>(
                typeof(SessionRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),
                typeof(StatusController));

            dependenciesController.AddDependencies<AccountProductsRecordsDataController>(
                typeof(AccountProductsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ApiProductsRecordsDataController>(
                typeof(ApiProductsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<GameDetailsRecordsDataController>(
                typeof(GameDetailsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<GameProductDataRecordsDataController>(
                typeof(GameProductDataRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductDownloadsRecordsDataController>(
                typeof(ProductDownloadsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductRoutesRecordsDataController>(
                typeof(ProductRoutesRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductScreenshotsRecordsDataController>(
                typeof(ProductScreenshotsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductsRecordsDataController>(
                typeof(ProductsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ValidationResultsRecordsDataController>(
                typeof(ValidationResultsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<UpdatedRecordsDataController>(
                typeof(UpdatedRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));             

            dependenciesController.AddDependencies<WishlistedRecordsDataController>(
                typeof(WishlistedRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(CollectionController),                
                typeof(StatusController));  

            // Controllers.Records.Session

            dependenciesController.AddDependencies<SessionRecordsIndexController>(
                typeof(SessionRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<SessionRecordsController>(
                typeof(SessionRecordsIndexController),
                typeof(ConvertStringToIndexDelegate));

            // Controllers.Records.ProductTypes

            dependenciesController.AddDependencies<AccountProductsRecordsIndexController>(
                typeof(AccountProductsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ApiProductsRecordsIndexController>(
                typeof(ApiProductsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameDetailsRecordsIndexController>(
                typeof(GameDetailsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameProductDataRecordsIndexController>(
                typeof(GameProductDataRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductDownloadsRecordsIndexController>(
                typeof(ProductDownloadsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductRoutesRecordsIndexController>(
                typeof(ProductRoutesRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductScreenshotsRecordsIndexController>(
                typeof(ProductScreenshotsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductsRecordsIndexController>(
                typeof(ProductsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ValidationResultsRecordsIndexController>(
                typeof(ValidationResultsRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<UpdatedRecordsIndexController>(
                typeof(UpdatedRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<WishlistedRecordsIndexController>(
                typeof(WishlistedRecordsDataController),
                typeof(StatusController));                                

            // GOG.Controllers.Stash.ProductTypes

            dependenciesController.AddDependencies<AccountProductsStashController>(
                typeof(GetAccountProductsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ApiProductsStashController>(
                typeof(GetApiProductsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameDetailsStashController>(
                typeof(GetGameDetailsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameProductDataStashController>(
                typeof(GetGameProductDataPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductsStashController>(
                typeof(GetProductsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            // Controllers.Stash.ProductTypes

            dependenciesController.AddDependencies<ProductDownloadsStashController>(
                typeof(GetProductDownloadsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductRoutesStashController>(
                typeof(GetProductRoutesPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductScreenshotsStashController>(
                typeof(GetProductScreenshotsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ValidationResultsStashController>(
                typeof(GetValidationResultsPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<UpdatedStashController>(
                typeof(GetUpdatedPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));     

            dependenciesController.AddDependencies<WishlistedStashController>(
                typeof(GetWishlistedPathDelegate),
                typeof(JSONSerializedStorageController),
                typeof(StatusController));                                

            // Controllers.Data.ProductTypes

            dependenciesController.AddDependencies<ProductDownloadsDataController>(
                typeof(ProductDownloadsStashController),
                typeof(ConvertProductDownloadsToIndexDelegate),
                typeof(ProductDownloadsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductRoutesDataController>(
                typeof(ProductRoutesStashController),
                typeof(ConvertProductRoutesToIndexDelegate),
                typeof(ProductRoutesRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductScreenshotsDataController>(
                typeof(ProductScreenshotsStashController),
                typeof(ConvertProductScreenshotsToIndexDelegate),
                typeof(ProductScreenshotsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ValidationResultsDataController>(
                typeof(ValidationResultsStashController),
                typeof(ConvertValidationResultsToIndexDelegate),
                typeof(ValidationResultsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<UpdatedDataController>(
                typeof(UpdatedStashController),
                typeof(ConvertPassthroughIndexDelegate),
                typeof(UpdatedRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));          

            dependenciesController.AddDependencies<WishlistedDataController>(
                typeof(WishlistedStashController),
                typeof(ConvertPassthroughIndexDelegate),
                typeof(WishlistedRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));                         

            // GOG.Controllers.Data.ProductTypes

            dependenciesController.AddDependencies<AccountProductsDataController>(
                typeof(AccountProductsStashController),
                typeof(ConvertAccountProductToIndexDelegate),
                typeof(AccountProductsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ApiProductsDataController>(
                typeof(ApiProductsStashController),
                typeof(ConvertApiProductToIndexDelegate),
                typeof(ApiProductsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<GameDetailsDataController>(
                typeof(GameDetailsStashController),
                typeof(ConvertGameDetailsToIndexDelegate),
                typeof(GameDetailsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<GameProductDataDataController>(
                typeof(GameProductDataStashController),
                typeof(ConvertGameProductDataToIndexDelegate),
                typeof(GameProductDataRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductsDataController>(
                typeof(ProductsStashController),
                typeof(ConvertProductToIndexDelegate),
                typeof(ProductsRecordsIndexController),
                typeof(CollectionController),                
                typeof(StatusController));

            // Delegates.Correct

            dependenciesController.AddDependencies<CorrectUsernamePasswordAsyncDelegate>(
                typeof(ConsoleInputOutputController));

            dependenciesController.AddDependencies<CorrectSecurityCodeAsyncDelegate>(
                typeof(ConsoleInputOutputController));

            // GOG.Controllers.Authorization

            dependenciesController.AddDependencies<GOGAuthorizationController>(
                typeof(CorrectUsernamePasswordAsyncDelegate),
                typeof(CorrectSecurityCodeAsyncDelegate),
                typeof(ItemizeLoginTokenAttributeValuesDelegate),
                typeof(ItemizeLoginIdAttributeValuesDelegate),
                typeof(ItemizeLoginUsernameAttributeValuesDelegate),
                typeof(ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate),
                typeof(UriController),
                typeof(NetworkController),
                typeof(JSONSerializationController),
                typeof(StatusController));

            // GOG.Activities.Authorize

            dependenciesController.AddDependencies<AuthorizeActivity>(
                typeof(GOGAuthorizationController),
                typeof(StatusController));

            // GOG.Delegates.GetPageResults

            dependenciesController.AddDependencies<GetProductsPageResultsAsyncDelegate>(
                typeof(GetProductsUpdateUriDelegate),
                typeof(GetProductsUpdateQueryParametersDelegate),
                typeof(RequestPageAsyncDelegate),
                typeof(JSONSerializationController),
                typeof(StatusController));

            // GOG.Activities.Update.ProductTypes

            dependenciesController.AddDependencies<UpdateProductsActivity>(
                typeof(GetProductsPageResultsAsyncDelegate),
                typeof(ItemizeProductsPageResultProductsDelegate),
                typeof(ProductsDataController),
                typeof(SessionRecordsController),
                typeof(StatusController));
            
            // GOG.Delegates.GetPageResults

            dependenciesController.AddDependencies<GetAccountProductsPageResultsAsyncDelegate>(
                typeof(GetAccountProductsUpdateUriDelegate),
                typeof(GetAccountProductsUpdateQueryParametersDelegate),
                typeof(RequestPageAsyncDelegate),
                typeof(JSONSerializationController),
                typeof(StatusController));

            // GOG.Activities.Update.ProductTypes

            dependenciesController.AddDependencies<UpdateAccountProductsActivity>(
                typeof(GetAccountProductsPageResultsAsyncDelegate),
                typeof(ItemizeAccountProductsPageResultProductsDelegate),
                typeof(AccountProductsDataController),
                typeof(SessionRecordsController),
                typeof(StatusController));

            dependenciesController.AddDependencies<UpdateUpdatedActivity>(
                typeof(AccountProductsDataController),
                typeof(ConfirmAccountProductUpdatedDelegate),
                typeof(UpdatedDataController),
                typeof(StatusController));

            // ...

            dependenciesController.AddDependencies<RecycleDelegate>(
                typeof(GetRecycleBinDirectoryDelegate),
                typeof(FileController),
                typeof(DirectoryController));

            dependenciesController.AddDependencies<ConfirmLikelyTokenTypeDelegate>(
                typeof(ArgsDefinitionsStashController),
                typeof(CollectionController));

            dependenciesController.AddDependencies<ConvertTokensToLikelyTypedTokensDelegate>(
                typeof(ConfirmLikelyTokenTypeDelegate));
        }
    }
}