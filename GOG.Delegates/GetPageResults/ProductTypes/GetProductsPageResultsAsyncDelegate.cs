using System.Collections.Generic;

using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Convert;

using Interfaces.Delegates.Activities;


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
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToProductsPageResultDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
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
