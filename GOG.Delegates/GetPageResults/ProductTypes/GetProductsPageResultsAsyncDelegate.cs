using System.Collections.Generic;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.RequestPage;
using GOG.Models;
using Delegates.GetValue.Uri.ProductTypes;
using Delegates.GetValue.QueryParameters.ProductTypes;
using Delegates.Activities;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            typeof(GetProductsUpdateUriDelegate),
            typeof(GetProductsUpdateQueryParametersDelegate),
            typeof(RequestPage.RequestPageAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToProductsPageResultDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public GetProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string> getProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getProductsQueryUpdateQueryParameters,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            IConvertDelegate<string, ProductsPageResult> convertJSONToProductsPageResultDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductsUpdateUriDelegate,
                getProductsQueryUpdateQueryParameters,
                requestPageAsyncDelegate,
                convertJSONToProductsPageResultDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}