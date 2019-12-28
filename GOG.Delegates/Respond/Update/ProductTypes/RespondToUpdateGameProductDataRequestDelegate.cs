using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Models;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method="update", Collection="gameproductdata")]
    public class RespondToUpdateGameProductDataRequestDelegate :
        RespondToUpdateMasterDetailsRequestDelegate<GameProductData, Product>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetGameProductDataUpdateUriDelegate,Delegates",
            "GOG.Delegates.Convert.UpdateIdentity.ConvertProductToGameProductDataUpdateIdentityDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.GameProductDataDataController,GOG.Controllers",
            "GOG.Delegates.Itemize.MasterDetail.ItemizeAllProductsGameProductDataGapsAsyncDelegatepsDelegate,GOG.Delegates",
            "GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedGameProductDataAsyncDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateGameProductDataRequestDelegate(
            IGetValueDelegate<string> getGameProductDataUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToGameProductDataUpdateIdentityDelegate,
            IDataController<GameProductData> gameProductDataDataController,
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsGameProductDataGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<GameProductData> getDeserializedGameProductDataAsyncDelegate,
            IActionLogController actionLogController):
            base(
                getGameProductDataUpdateUriDelegate,
                convertProductToGameProductDataUpdateIdentityDelegate,
                gameProductDataDataController,
                itemizeAllProductsGameProductDataGapsAsyncDelegate,
                getDeserializedGameProductDataAsyncDelegate,
                actionLogController,
                null)
                {
                    // ...
                }
    }
}
