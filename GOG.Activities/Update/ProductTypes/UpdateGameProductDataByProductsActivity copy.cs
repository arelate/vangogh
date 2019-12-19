using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

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
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdateApiProductsByProductsActivity(
            IGetValueDelegate<string> getApiProductsUpdateUriDelegate,
            IConvertDelegate<Product, string> convertProductToApiProductUpdateIdentityDelegate,
            IDataController<ApiProduct> apiProductsDataController,
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsApiProductsGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<ApiProduct> getDeserializedApiProductAsyncDelegate,
            IActionLogController actionLogController):
            base(
                getApiProductsUpdateUriDelegate,
                convertProductToApiProductUpdateIdentityDelegate,
                apiProductsDataController,
                itemizeAllProductsApiProductsGapsAsyncDelegate,
                getDeserializedApiProductAsyncDelegate,
                actionLogController,
                null)
                {
                    // ...
                }
    }
}
