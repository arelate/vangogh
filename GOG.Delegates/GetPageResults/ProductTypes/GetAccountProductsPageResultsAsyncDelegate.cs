using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Logs;


using Attributes;

using GOG.Interfaces.Delegates.RequestPage;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetAccountProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<AccountProductsPageResult>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetAccountProductsUpdateUriDelegate,Delegates",
            "Delegates.GetValue.QueryParameters.ProductTypes.GetAccountProductsUpdateQueryParametersDelegate,Delegates",
            "GOG.Delegates.RequestPage.RequestPageAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToAccountProductsPageResultDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GetAccountProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string> getAccountProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getAccountProductsQueryUpdateQueryParameters,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            IConvertDelegate<string, AccountProductsPageResult> convertJSONToAccountProductsPageResultDelegate,
            IActionLogController actionLogController) :
            base(
                getAccountProductsUpdateUriDelegate,
                getAccountProductsQueryUpdateQueryParameters,
                requestPageAsyncDelegate,
                convertJSONToAccountProductsPageResultDelegate,
                actionLogController)
        {
            // ...
        }
    }
}
