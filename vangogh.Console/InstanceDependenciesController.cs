using Interfaces.Controllers.Dependencies;

using Controllers.Collection;
using Controllers.Stash.ArgsDefinitions;
using Controllers.Stash.Hashes;
using Controllers.Stash.Templates;
using Controllers.Stash.Cookies;
using Controllers.Stash.Records;
using Controllers.Hashes;
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
using Controllers.Records.Session;

using Delegates.GetDirectory.Root;
using Delegates.GetDirectory.Data;
using Delegates.GetFilename;
using Delegates.GetFilename.ArgsDefinitions;
using Delegates.GetFilename.Binary;
using Delegates.GetFilename.Json;
using Delegates.GetFilename.Data;
using Delegates.GetPath;
using Delegates.GetPath.ArgsDefinitions;
using Delegates.GetPath.Binary;
using Delegates.GetPath.Json;
using Delegates.GetPath.Records;
using Delegates.GetPath.Data;
using Delegates.Convert;
using Delegates.Convert.Hashes;
using Delegates.Convert.Bytes;
using Delegates.Convert.Collections.Status;
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

using GOG.Delegates.Itemize;
using GOG.Delegates.RequestPage;

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

            dependenciesController.AddDependencies<GetHashesFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetAppTemplateFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetReportTemplateFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetCookiesFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetIndexFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetWishlistedFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetUpdatedFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetSessionRecordsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetAccountProductsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetApiProductsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetGameDetailsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetGameProductDataFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetProductDownloadsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetProductRoutesFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetProductScreenshotsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetProductsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            dependenciesController.AddDependencies<GetValidationResultsFilenameDelegate>(
                typeof(GetBinFilenameDelegate));

            // Delegates.GetPath

            dependenciesController.AddDependencies<GetArgsDefinitionsPathDelegate>(
                typeof(GetEmptyDirectoryDelegate),
                typeof(GetArgsDefinitionsFilenameDelegate));

            dependenciesController.AddDependencies<GetHashesPathDelegate>(
                typeof(GetEmptyDirectoryDelegate),
                typeof(GetHashesFilenameDelegate));

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

            // Delegates.GetPath.Data

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

            // Controllers.Stash

            dependenciesController.AddDependencies<HashesStashController>(
                typeof(GetHashesPathDelegate),
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            // Controllers.Stash.Records

            dependenciesController.AddDependencies<SessionRecordsStashController>(
                typeof(GetSessionRecordsPathDelegate),
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<AccountProductsRecordsStashController>(
                typeof(GetAccountProductsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ApiProductsRecordsStashController>(
                typeof(GetApiProductsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameDetailsRecordsStashController>(
                typeof(GetGameDetailsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<GameProductDataRecordsStashController>(
                typeof(GetGameProductDataRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductDownloadsRecordsStashController>(
                typeof(GetProductDownloadsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductRoutesRecordsStashController>(
                typeof(GetProductRoutesRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductScreenshotsRecordsStashController>(
                typeof(GetProductScreenshotsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ProductsRecordsStashController>(
                typeof(GetProductsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            dependenciesController.AddDependencies<ValidationResultsRecordsStashController>(
                typeof(GetValidationResultsRecordsPathDelegate),                
                typeof(ProtoBufSerializedStorageController),
                typeof(StatusController));

            // Controllers.Hashes

            dependenciesController.AddDependencies<HashesController>(
                typeof(HashesStashController));

            // Controllers.SerializedStorage.JSON

            dependenciesController.AddDependencies<JSONSerializedStorageController>(
                typeof(HashesController),
                typeof(StorageController),
                typeof(ConvertStringToMd5HashDelegate),
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
                typeof(StatusController),
                typeof(HashesController));

            dependenciesController.AddDependencies<AccountProductsRecordsDataController>(
                typeof(AccountProductsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<ApiProductsRecordsDataController>(
                typeof(ApiProductsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));  

            dependenciesController.AddDependencies<GameDetailsRecordsDataController>(
                typeof(GameDetailsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<GameProductDataRecordsDataController>(
                typeof(GameProductDataRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<ProductDownloadsRecordsDataController>(
                typeof(ProductDownloadsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<ProductRoutesRecordsDataController>(
                typeof(ProductRoutesRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<ProductScreenshotsRecordsDataController>(
                typeof(ProductScreenshotsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<ProductsRecordsDataController>(
                typeof(ProductsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            dependenciesController.AddDependencies<ValidationResultsRecordsDataController>(
                typeof(ValidationResultsRecordsStashController),
                typeof(ConvertProductCoreToIndexDelegate<ProductRecords>),
                typeof(StatusController),
                typeof(HashesController));                

            // Controllers.Records.Session

            dependenciesController.AddDependencies<SessionRecordsIndexController>(
                typeof(SessionRecordsDataController),
                typeof(StatusController));

            dependenciesController.AddDependencies<SessionRecordsController>(
                typeof(SessionRecordsIndexController),
                typeof(ConvertStringToIndexDelegate));

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