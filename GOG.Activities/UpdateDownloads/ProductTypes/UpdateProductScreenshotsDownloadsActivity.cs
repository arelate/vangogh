using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;
using Interfaces.Controllers.Logs;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;
using Models.ProductTypes;

namespace GOG.Activities.UpdateDownloads.ProductTypes
{
    public class UpdateProductScreenshotsDownloadsActivity : UpdateDownloadsActivity<ProductScreenshots>
    {
        [Dependencies(
            "GOG.Delegates.GetDownloadSources.GetScreenshotsDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetScreenshotsDirectoryDelegate,Delegates",
            "Controllers.File.FileController,Controllers",
            "Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public UpdateProductScreenshotsDownloadsActivity(
            IGetDownloadSourcesAsyncDelegate getProductScreenshotsDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductScreenshotsDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IResponseLogController responseLogController) :
            base(
                getProductScreenshotsDownloadSourcesAsyncDelegate,
                getProductScreenshotsDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                responseLogController)
        {
            // ...
        }
    }
}
