using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;

using Interfaces.Status;

using Models.ProductDownloads;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;

namespace GOG.Activities.UpdateDownloads.ProductTypes
{
    public class UpdateProductImagesDownloadsActivity : UpdateDownloadsActivity<Product>
    {
		[Dependencies(
			"GOG.Delegates.GetDownloadSources.ProductTypes.GetProductImagesDownloadSourcesAsyncDelegate,GOG.Delegates",
			"Delegates.GetDirectory.ProductTypes.GetProductImagesDirectoryDelegate,Delegates",
			"Controllers.File.FileController,Controllers",
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
			"GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
			"Controllers.Status.StatusController,Controllers")]
        public UpdateProductImagesDownloadsActivity(
            IGetDownloadSourcesAsyncDelegate getProductImagesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductImagesDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IStatusController statusController) :
            base(
                getProductImagesDownloadSourcesAsyncDelegate,
                getProductImagesDirectoryDelegate,
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
