using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;

using Interfaces.Status;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;

namespace GOG.Activities.UpdateDownloads.ProductTypes
{
    public class UpdateProductFilesDownloadsActivity : UpdateDownloadsActivity<ProductFile>
    {
		[Dependencies(
			"GOG.Delegates.GetDownloadSources.GetManualUrlDownloadSourcesAsyncDelegate,GOG.Delegates",
			"Delegates.GetDirectory.ProductTypes.GetProductFilesDirectoryDelegate,Delegates",
			"Controllers.File.FileController,Controllers",
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
			"GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
			"Controllers.Status.StatusController,Controllers")]        
        public UpdateProductFilesDownloadsActivity(
            IGetDownloadSourcesAsyncDelegate getProductFilesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductFilesDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IStatusController statusController) :
            base(
                getProductFilesDownloadSourcesAsyncDelegate,
                getProductFilesDirectoryDelegate,
                fileController,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                statusController)
        {
            // ...
        }
    }
}
