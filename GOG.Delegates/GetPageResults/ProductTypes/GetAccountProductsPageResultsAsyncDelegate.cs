using System.Collections.Generic;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.RequestPage;
using GOG.Models;
using Delegates.GetValue.Uri.ProductTypes;
using Delegates.GetValue.QueryParameters.ProductTypes;
using Delegates.Activities;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetAccountProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<AccountProductsPageResult>
    {
        [Dependencies(
            typeof(GetAccountProductsUpdateUriDelegate),
            typeof(GetAccountProductsUpdateQueryParametersDelegate),
            typeof(RequestPage.RequestPageAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToAccountProductsPageResultDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public GetAccountProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string> getAccountProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getAccountProductsQueryUpdateQueryParameters,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            IConvertDelegate<string, AccountProductsPageResult> convertJSONToAccountProductsPageResultDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getAccountProductsUpdateUriDelegate,
                getAccountProductsQueryUpdateQueryParameters,
                requestPageAsyncDelegate,
                convertJSONToAccountProductsPageResultDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}