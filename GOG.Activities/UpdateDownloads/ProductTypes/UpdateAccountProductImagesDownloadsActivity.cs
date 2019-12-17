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
    public class UpdateAccountProductImagesDownloadsActivity : UpdateDownloadsActivity<AccountProduct>
    {
		[Dependencies(
			"GOG.Delegates.GetDownloadSources.ProductTypes.GetAccountProductImagesDownloadSourcesAsyncDelegate,GOG.Delegates",
			"Delegates.GetDirectory.ProductTypes.GetAccountProductImagesDirectoryDelegate,Delegates",
			"Controllers.File.FileController,Controllers",
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
			"GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
			"Controllers.Status.StatusController,Controllers")]        
        public UpdateAccountProductImagesDownloadsActivity(
            IGetDownloadSourcesAsyncDelegate getAccountProductImagesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getAccountProductImagesDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IStatusController statusController) :
            base(
                getAccountProductImagesDownloadSourcesAsyncDelegate,
                getAccountProductImagesDirectoryDelegate,
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
