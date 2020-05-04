using Attributes;
using Delegates.Activities;
using Delegates.Format.Uri;
using Delegates.Itemize.ProductTypes;
using GOG.Delegates.Values.Images;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Values;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeAllProductImagesDownloadSourcesAsyncDelegate :
        ItemizeAllProductCoreImagesDownloadSourcesAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(FormatImagesUriDelegate),
            typeof(GetProductImageUriDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllProductImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetValueDelegate<string, Product> getProductImageUriDelegate,
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