using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Hashes;

using Interfaces.Controllers.Serialization;
using Interfaces.Status;

using GOG.Interfaces.Delegates.RequestPage;

using GOG.Models;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<ProductsPageResult>
    {
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
