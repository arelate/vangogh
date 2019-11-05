using Interfaces.Controllers.Dependencies;

using Controllers.Collection;
using Controllers.Stash.ArgsDefinitions;
using Controllers.Stash.Hashes;
using Controllers.Stash.Templates;
using Controllers.Stash.Cookies;
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

using Delegates.GetDirectory.Root;
using Delegates.GetDirectory.Data;
using Delegates.GetFilename;
using Delegates.GetFilename.ArgsDefinitions;
using Delegates.GetFilename.Binary;
using Delegates.GetFilename.Json;
using Delegates.GetPath;
using Delegates.GetPath.ArgsDefinitions;
using Delegates.GetPath.Binary;
using Delegates.GetPath.Json;
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

            // Controllers.Stash.Hashes

            dependenciesController.AddDependencies<HashesStashController>(
                typeof(GetHashesPathDelegate),
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