using System.Collections.Generic;

using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Serialization;
using Interfaces.Status;

using Attributes;

using GOG.Interfaces.Delegates.RequestPage;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetProductsUpdateUriDelegate,Delegates",
            "Delegates.GetValue.QueryParameters.ProductTypes.GetProductsUpdateQueryParametersDelegate,Delegates",
            "GOG.Delegates.RequestPage.RequestPageAsyncDelegate,GOG.Delegates",
            Dependencies.JSONSerializationController,
            "Controllers.Status.StatusController,Controllers")]
        public GetProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string> getProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getProductsQueryUpdateQueryParameters,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            ISerializationController<string> serializationController,
            IStatusController statusController):
            base(
                getProductsUpdateUriDelegate,
                getProductsQueryUpdateQueryParameters,
                requestPageAsyncDelegate,
                serializationController,
                statusController)
        {
            // ...
        }
    }
}
