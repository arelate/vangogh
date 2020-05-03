using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Models;
using Delegates.Activities;
using Delegates.Values.Uri.ProductTypes;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "apiproducts")]
    public class RespondToUpdateApiProductsRequestDelegate :
        RespondToUpdateMasterDetailsRequestDelegate<ApiProduct, Product>
    {
        [Dependencies(
            typeof(GetApiProductsUpdateUriDelegate),
            typeof(Convert.UpdateIdentity.ConvertProductToApiProductUpdateIdentityDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateApiProductsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitApiProductsAsyncDelegate),
            typeof(Itemize.MasterDetail.ItemizeAllProductsApiProductsGapsAsyncDelegate),
            typeof(GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedApiProductAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateApiProductsRequestDelegate(
            IGetValueDelegate<string, string> getApiProductsUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToApiProductUpdateIdentityDelegate,
            IUpdateAsyncDelegate<ApiProduct> updateApiProductsAsyncDelegate,
            ICommitAsyncDelegate commitApiProductsAsyncDelegate,
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsApiProductsGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<ApiProduct> getDeserializedApiProductAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getApiProductsUpdateUriDelegate,
                convertProductToApiProductUpdateIdentityDelegate,
                updateApiProductsAsyncDelegate,
                commitApiProductsAsyncDelegate,
                itemizeAllProductsApiProductsGapsAsyncDelegate,
                getDeserializedApiProductAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate,
                null)
        {
            // ...
        }
    }
}