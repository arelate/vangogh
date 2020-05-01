using System.Collections.Generic;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
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
            "Delegates.Itemize.ProductTypes.ItemizeAllUpdatedAsyncDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate,GOG.Delegates",
            "Delegates.Format.Uri.FormatImagesUriDelegate,Delegates",
            "GOG.Delegates.GetImageUri.GetAccountProductImageUriDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GetAccountProductImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<AccountProduct> getAccountProductImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllUpdatedAsyncDelegate,
                getAccountProductByIdAsyncDelegate,
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