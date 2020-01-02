using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;
using Interfaces.Controllers.Logs;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "accountproductimages")]
    public class RespondToUpdateAccountProductImagesDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<AccountProductImage>
    {
        [Dependencies(
            "GOG.Delegates.GetDownloadSources.ProductTypes.GetAccountProductImagesDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetAccountProductImagesDirectoryDelegate,Delegates",
            "Controllers.File.FileController,Controllers",
            "Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateAccountProductImagesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getAccountProductImagesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getAccountProductImagesDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IActionLogController actionLogController) :
            base(
                getAccountProductImagesDownloadSourcesAsyncDelegate,
                getAccountProductImagesDirectoryDelegate,
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
