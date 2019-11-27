using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Serialization;
using Interfaces.Status;

using Attributes;

using GOG.Interfaces.Delegates.RequestPage;

using GOG.Models;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetAccountProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<AccountProductsPageResult>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetAccountProductsUpdateUriDelegate,Delegates",
            "Delegates.GetValue.QueryParameters.ProductTypes.GetAccountProductsUpdateQueryParametersDelegate,Delegates",
            "GOG.Delegates.RequestPage.RequestPageAsyncDelegate,GOG.Delegates",
            "Controllers.Serialization.JSON.JSONSerializationController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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
