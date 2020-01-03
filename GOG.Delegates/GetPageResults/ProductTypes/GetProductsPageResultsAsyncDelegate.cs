using System.Collections.Generic;

using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using GOG.Interfaces.Delegates.RequestPage;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetValue.Uri.ProductTypes.GetProductsUpdateUriDelegate,Delegates",
            "Delegates.GetValue.QueryParameters.ProductTypes.GetProductsUpdateQueryParametersDelegate,Delegates",
            "GOG.Delegates.RequestPage.RequestPageAsyncDelegate,GOG.Delegates",
            Dependencies.JSONSerializationController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public GetProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string> getProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getProductsQueryUpdateQueryParameters,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            ISerializationController<string> serializationController,
            IActionLogController actionLogController):
            base(
                getProductsUpdateUriDelegate,
                getProductsQueryUpdateQueryParameters,
                requestPageAsyncDelegate,
                serializationController,
                actionLogController)
        {
            // ...
        }
    }
}
