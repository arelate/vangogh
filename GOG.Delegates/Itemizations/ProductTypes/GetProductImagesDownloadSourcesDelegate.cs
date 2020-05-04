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
    public class ItemizeAllProductImagesDownloadSourcesAsyncDelegate :
        ItemizeAllProductCoreImagesDownloadSourcesAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(ConvertImagesUriTemplateToUriDelegate),
            typeof(GetProductImageUriDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllProductImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IConvertDelegate<string, string> convertImagesUriTemplateToUriDelegate,
            IGetValueDelegate<string, Product> getProductImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllUpdatedAsyncDelegate,
                getProductByIdAsyncDelegate,
                convertImagesUriTemplateToUriDelegate,
                getProductImageUriDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}