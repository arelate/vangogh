using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Data;

using Attributes;

using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Models;

namespace GOG.Activities.Update
{
    public class UpdateGameProductDataByProductsActivity :
        UpdateDetailProductsByMasterProductsActivity<GameProductData, Product>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetGameProductDataUpdateUriDelegate,Delegates",
            "GOG.Delegates.Convert.UpdateIdentity.ConvertProductToGameProductDataUpdateIdentityDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.GameProductDataDataController,GOG.Controllers",
            "Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
            "GOG.Delegates.GetDeserialized.GetDeserializedGameProductDataAsyncDelegate,GOG.Delegates",
            "Controllers.Status.StatusController,Controllers")]
        public UpdateGameProductDataByProductsActivity(
            IGetValueDelegate<string> getGameProductDataUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToGameProductDataUpdateIdentityDelegate,
            IDataController<Product> productsDataController,
            IDataController<GameProductData> gameProductDataDataController,
            IDataController<long> updatedDataController,
            IGetDeserializedAsyncDelegate<GameProductData> getDeserializedGameProductDataAsyncDelegate,
            IStatusController statusController):
            base(
                getGameProductDataUpdateUriDelegate,
                convertProductToGameProductDataUpdateIdentityDelegate,
                productsDataController,
                gameProductDataDataController,
                updatedDataController,
                getDeserializedGameProductDataAsyncDelegate,
                statusController,
                null)
                {
                    // ...
                }
    }
}
