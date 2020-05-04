using Attributes;
using Delegates.Activities;
using Delegates.Values.Uri.ProductTypes;
using GOG.Delegates.Conversions.UpdateIdentity;
using GOG.Delegates.Data.Models.ProductTypes;
using GOG.Delegates.Itemizations.MasterDetail;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;

namespace GOG.Delegates.Server.Update
{
    [RespondsToRequests(Method = "update", Collection = "gameproductdata")]
    public class UpdateGameProductDataAsyncDelegate :
        UpdateMasterDetailsAsyncDelegate<GameProductData, Product>
    {
        [Dependencies(
            typeof(GetGameProductDataUpdateUriDelegate),
            typeof(ConvertProductToGameProductDataUpdateIdentityDelegate),
            typeof(UpdateGameProductDataAsyncDelegate),
            typeof(CommitGameProductDataAsyncDelegate),
            typeof(ItemizeAllProductsGameProductDataGapsAsyncDelegate),
            typeof(GetDeserializedGameProductDataAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateGameProductDataAsyncDelegate(
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