using System.Collections.Generic;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
using Attributes;
using GOG.Interfaces.Delegates.GetImageUri;
using GOG.Models;

namespace GOG.Delegates.GetDownloadSources.ProductTypes
{
    public class GetProductImagesDownloadSourcesAsyncDelegate :
        GetProductCoreImagesDownloadSourcesAsyncDelegate<Product>
    {
        [Dependencies(
            "Delegates.Itemize.ProductTypes.ItemizeAllUpdatedAsyncDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate,GOG.Delegates",
            "Delegates.Format.Uri.FormatImagesUriDelegate,Delegates",
            "GOG.Delegates.GetImageUri.GetProductImageUriDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GetProductImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<Product> getProductImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllUpdatedAsyncDelegate,
                getProductByIdAsyncDelegate,
                formatImagesUriDelegate,
                getProductImageUriDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}