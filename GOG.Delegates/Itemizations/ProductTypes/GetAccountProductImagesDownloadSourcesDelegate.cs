using Attributes;
using Delegates.Activities;
using Delegates.Conversions.Uris;
using Delegates.Itemizations.ProductTypes;
using GOG.Delegates.Values.Images;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;

namespace GOG.Delegates.Itemizations.ProductTypes
{
    public class ItemizeAllAccountProductImagesDownloadSourcesAsyncDelegate :
        ItemizeAllProductCoreImagesDownloadSourcesAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(ConvertImagesUriTemplateToUriDelegate),
            typeof(GetAccountProductImageUriDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllAccountProductImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IConvertDelegate<string, string> convertImagesUriTemplateToUriDelegate,
            IGetValueDelegate<string, AccountProduct> getAccountProductImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllUpdatedAsyncDelegate,
                getAccountProductByIdAsyncDelegate,
                convertImagesUriTemplateToUriDelegate,
                getAccountProductImageUriDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}