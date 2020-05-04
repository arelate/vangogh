using Interfaces.Delegates.Format;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Activities;
using Attributes;
using Interfaces.Delegates.Values;
using GOG.Models;
using Delegates.Itemize.ProductTypes;
using Delegates.Format.Uri;
using Delegates.Activities;
using GOG.Delegates.Values.Images;

namespace GOG.Delegates.GetDownloadSources.ProductTypes
{
    public class GetAccountProductImagesDownloadSourcesAsyncDelegate :
        GetProductCoreImagesDownloadSourcesAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(FormatImagesUriDelegate),
            typeof(GetAccountProductImageUriDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public GetAccountProductImagesDownloadSourcesAsyncDelegate(
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