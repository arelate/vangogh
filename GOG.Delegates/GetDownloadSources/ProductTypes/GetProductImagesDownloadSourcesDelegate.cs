using System.Collections.Generic;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using GOG.Interfaces.Delegates.GetImageUri;

using GOG.Models;

namespace GOG.Delegates.GetDownloadSources.ProductTypes
{
    public class GetProductImagesDownloadSourcesAsyncDelegate :
        GetProductCoreImagesDownloadSourcesAsyncDelegate<Product>
    {
        [Dependencies(
            "Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Delegates.Format.Uri.FormatImagesUriDelegate,Delegates",
            "GOG.Delegates.GetImageUri.GetProductImageUriDelegate,GOG.Delegates",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public GetProductImagesDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<Product> productsDataController,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<Product> getProductImageUriDelegate,
            IActionLogController actionLogController) :
            base(
                updatedDataController,
                productsDataController,
                formatImagesUriDelegate,
                getProductImageUriDelegate,
                actionLogController)
        {
            // ...
        }

    }
}
