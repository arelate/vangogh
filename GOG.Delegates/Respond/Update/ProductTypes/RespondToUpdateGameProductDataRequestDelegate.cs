using Interfaces.Delegates.Values;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;
using Delegates.Activities;
using Delegates.Values.Uri.ProductTypes;
using GOG.Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "gameproductdata")]
    public class RespondToUpdateGameProductDataRequestDelegate :
        RespondToUpdateMasterDetailsRequestDelegate<GameProductData, Product>
    {
        [Dependencies(
            typeof(GetGameProductDataUpdateUriDelegate),
            typeof(Convert.UpdateIdentity.ConvertProductToGameProductDataUpdateIdentityDelegate),
            typeof(UpdateGameProductDataAsyncDelegate),
            typeof(CommitGameProductDataAsyncDelegate),
            typeof(Itemize.MasterDetail.ItemizeAllProductsGameProductDataGapsAsyncDelegate),
            typeof(GetDeserializedGameProductDataAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateGameProductDataRequestDelegate(
            IGetValueDelegate<string,string> getGameProductDataUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToGameProductDataUpdateIdentityDelegate,
            IUpdateAsyncDelegate<GameProductData> updateGameProductDataAsyncDelegate,
            ICommitAsyncDelegate commitGameProductDataAsyncDelegate,
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsGameProductDataGapsAsyncDelegate,
            IGetDataAsyncDelegate<GameProductData, string> getDeserializedGameProductDataAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getGameProductDataUpdateUriDelegate,
                convertProductToGameProductDataUpdateIdentityDelegate,
                updateGameProductDataAsyncDelegate,
                commitGameProductDataAsyncDelegate,
                itemizeAllProductsGameProductDataGapsAsyncDelegate,
                getDeserializedGameProductDataAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}