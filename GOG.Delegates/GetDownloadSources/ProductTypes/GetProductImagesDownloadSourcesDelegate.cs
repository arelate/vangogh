using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
using Attributes;
using GOG.Interfaces.Delegates.GetImageUri;
using GOG.Models;
using Delegates.Format.Uri;
using Delegates.Activities;
using Delegates.Itemize.ProductTypes;

namespace GOG.Delegates.GetDownloadSources.ProductTypes
{
    public class GetProductImagesDownloadSourcesAsyncDelegate :
        GetProductCoreImagesDownloadSourcesAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(FormatImagesUriDelegate),
            typeof(GetImageUri.GetProductImageUriDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
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