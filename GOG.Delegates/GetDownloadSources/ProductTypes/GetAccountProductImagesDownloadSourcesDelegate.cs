using System.Collections.Generic;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using Attributes;

using GOG.Interfaces.Delegates.GetImageUri;

using GOG.Models;

namespace GOG.Delegates.GetDownloadSources.ProductTypes
{
    public class GetAccountProductImagesDownloadSourcesAsyncDelegate : 
        GetProductCoreImagesDownloadSourcesAsyncDelegate<AccountProduct>
    {
        [Dependencies(
			"Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
			"Delegates.Format.Uri.FormatImagesUriDelegate,Delegates",
			"GOG.Delegates.GetImageUri.GetAccountProductImageUriDelegate,GOG.Delegates",
			"Controllers.Status.StatusController,Controllers")]
        public GetAccountProductImagesDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<AccountProduct> getAccountProductImageUriDelegate,
            IStatusController statusController):
            base(
                updatedDataController,
                accountProductsDataController,
                formatImagesUriDelegate,
                getAccountProductImageUriDelegate,
                statusController)
        {
            // ...
        }

    }
}
