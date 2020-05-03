using System.Collections.Generic;

namespace Delegates.Values.QueryParameters.ProductTypes
{
    public class GetAccountProductsUpdateQueryParametersDelegate : GetConstValueDelegate<Dictionary<string, string>>
    {
        public GetAccountProductsUpdateQueryParametersDelegate() :
            base(Models.QueryParameters.QueryParametersCollections.AccountGetFilteredProducts)
        {
            // ...
        }
    }
}