using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;
using Models.ProductTypes;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    // TODO: productscreenshots or just screenshots?
    [RespondsToRequests(Method = "updatedownloads", Collection = "productscreenshots")]
    public class RespondToUpdateProductScreenshotsDownloadsRequestDelegate : 
        RespondToUpdateDownloadsRequestDelegate<ProductScreenshots>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Delegates.GetDownloadSources.GetScreenshotsDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetScreenshotsDirectoryDelegate,Delegates",
            "Controllers.File.FileController,Controllers",
            "Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateProductScreenshotsDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductScreenshotsDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductScreenshotsDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IActionLogController actionLogController) :
            base(
                getProductScreenshotsDownloadSourcesAsyncDelegate,
                getProductScreenshotsDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                actionLogController)
        {
            // ...
        }
    }
}
