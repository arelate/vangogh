using System.Collections.Generic;

namespace Delegates.GetValue.QueryParameters.ProductTypes
{
    public class GetProductsUpdateQueryParametersDelegate : GetConstValueDelegate<Dictionary<string, string>>
    {
        public GetProductsUpdateQueryParametersDelegate() :
            base(Models.QueryParameters.QueryParametersCollections.GamesAjaxFiltered)
        {
            // ...
        }
    }
}