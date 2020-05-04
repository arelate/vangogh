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
    public class ItemizeAllAccountProductImagesDownloadSourcesAsyncDelegate :
        ItemizeAllProductCoreImagesDownloadSourcesAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(FormatImagesUriDelegate),
            typeof(GetAccountProductImageUriDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllAccountProductImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetValueDelegate<string, AccountProduct> getAccountProductImageUriDelegate,
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