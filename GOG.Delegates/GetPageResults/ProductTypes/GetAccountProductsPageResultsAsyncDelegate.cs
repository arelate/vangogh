using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Serialization;
using Interfaces.Status;

using GOG.Interfaces.Delegates.RequestPage;

using GOG.Models;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetAccountProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<AccountProductsPageResult>
    {
        public GetAccountProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string> getAccountProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getAccountProductsQueryUpdateQueryParameters,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            ISerializationController<string> serializationController,
            IStatusController statusController) :
            base(
                getAccountProductsUpdateUriDelegate,
                getAccountProductsQueryUpdateQueryParameters,
                requestPageAsyncDelegate,
                serializationController,
                statusController)
        {
            // ...
        }
    }
}
