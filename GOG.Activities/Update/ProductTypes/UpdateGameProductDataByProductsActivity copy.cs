using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Attributes;

using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Models;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateApiProductsByProductsActivity :
        UpdateDetailProductsByMasterProductsActivity<ApiProduct, Product>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetApiProductsUpdateUriDelegate,Delegates",
            "GOG.Delegates.Convert.UpdateIdentity.ConvertProductToApiProductUpdateIdentityDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.ApiProductsDataController,GOG.Controllers",
            "GOG.Delegates.Itemize.MasterDetail.ItemizeAllProductsApiProductsGapsAsyncDelegatepsDelegate,GOG.Delegates",
            "GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedApiProductAsyncDelegate,GOG.Delegates",
            "Controllers.Status.StatusController,Controllers")]
        public UpdateApiProductsByProductsActivity(
            IGetValueDelegate<string> getApiProductsUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToApiProductUpdateIdentityDelegate,
            IDataController<ApiProduct> apiProductsDataController,
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsApiProductsGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<ApiProduct> getDeserializedApiProductAsyncDelegate,
            IStatusController statusController):
            base(
                getApiProductsUpdateUriDelegate,
                convertProductToApiProductUpdateIdentityDelegate,
                apiProductsDataController,
                itemizeAllProductsApiProductsGapsAsyncDelegate,
                getDeserializedApiProductAsyncDelegate,
                statusController,
                null)
                {
                    // ...
                }
    }
}
