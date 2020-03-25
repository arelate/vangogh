using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;


using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "productfiles")]
    public class RespondToUpdateProductFilesDownloadsRequestDelegate : 
        RespondToUpdateDownloadsRequestDelegate<ProductFile>
    {
		[Dependencies(
			"GOG.Delegates.GetDownloadSources.GetManualUrlDownloadSourcesAsyncDelegate,GOG.Delegates",
			"Delegates.GetDirectory.ProductTypes.GetProductFilesDirectoryDelegate,Delegates",
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
			"GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
			"Controllers.Logs.ActionLogController,Controllers")]        
        public RespondToUpdateProductFilesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductFilesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductFilesDirectoryDelegate,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IActionLogController actionLogController) :
            base(
                getProductFilesDownloadSourcesAsyncDelegate,
                getProductFilesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                actionLogController)
        {
            // ...
        }
    }
}
