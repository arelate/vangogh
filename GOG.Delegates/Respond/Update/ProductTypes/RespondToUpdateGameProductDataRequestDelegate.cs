using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Models;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "gameproductdata")]
    public class RespondToUpdateGameProductDataRequestDelegate :
        RespondToUpdateMasterDetailsRequestDelegate<GameProductData, Product>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetGameProductDataUpdateUriDelegate,Delegates",
            "GOG.Delegates.Convert.UpdateIdentity.ConvertProductToGameProductDataUpdateIdentityDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.UpdateGameProductDataAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.CommitGameProductDataAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.MasterDetail.ItemizeAllProductsGameProductDataGapsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedGameProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateGameProductDataRequestDelegate(
            IGetValueDelegate<string> getGameProductDataUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToGameProductDataUpdateIdentityDelegate,
            IUpdateAsyncDelegate<GameProductData> updateGameProductDataAsyncDelegate,
            ICommitAsyncDelegate commitGameProductDataAsyncDelegate,
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsGameProductDataGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<GameProductData> getDeserializedGameProductDataAsyncDelegate,
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
                completeDelegate,
                null)
        {
            // ...
        }
    }
}