using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;


using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "productimages")]
    public class RespondToUpdateProductImagesDownloadsRequestDelegate : 
        RespondToUpdateDownloadsRequestDelegate<ProductImage>
    {
        [Dependencies(
            "GOG.Delegates.GetDownloadSources.ProductTypes.GetProductImagesDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetProductImagesDirectoryDelegate,Delegates",
            "Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateProductImagesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductImagesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductImagesDirectoryDelegate,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IActionLogController actionLogController) :
            base(
                getProductImagesDownloadSourcesAsyncDelegate,
                getProductImagesDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                productsDataController,
                actionLogController)
        {
            // ...
        }
    }
}
