using Interfaces.Controllers.Dependencies;

using Controllers.Collection;
using Controllers.Stash.ArgsDefinitions;

using Delegates.GetFilename;
using Delegates.GetFilename.ArgsDefinitions;
using Delegates.GetFilename.Binary;
using Delegates.GetFilename.Json;

using Delegates.GetDirectory.Root;
using Delegates.GetDirectory.Data;

using Delegates.Confirm.ArgsTokens;
using Delegates.Convert.ArgsTokens;

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

            dependenciesController.AddDependencies<GetReportTemplateFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetIndexFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));

            dependenciesController.AddDependencies<GetWishlistedFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));                

            dependenciesController.AddDependencies<GetUpdatedFilenameDelegate>(
                typeof(GetJsonFilenameDelegate));     





            dependenciesController.AddDependencies<ConfirmLikelyTokenTypeDelegate>(
                typeof(ArgsDefinitionsStashController),
                typeof(CollectionController));
            
            dependenciesController.AddDependencies<ConvertTokensToLikelyTypedTokensDelegate>(
                typeof(ConfirmLikelyTokenTypeDelegate));
        }
    }
}