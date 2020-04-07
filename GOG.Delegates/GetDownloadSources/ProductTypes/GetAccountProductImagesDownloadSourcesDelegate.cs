using System.Collections.Generic;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


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
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GetAccountProductImagesDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<AccountProduct> getAccountProductImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate):
            base(
                updatedDataController,
                accountProductsDataController,
                formatImagesUriDelegate,
                getAccountProductImageUriDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }

    }
}
