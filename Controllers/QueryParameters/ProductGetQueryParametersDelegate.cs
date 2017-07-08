using System;
using System.Collections.Generic;

using Interfaces.QueryParameters;
using Interfaces.ContextDefinitions;

namespace Controllers.QueryParameters
{
    public class ProductParameterGetQueryParametersDelegate : IGetQueryParametersDelegate<Context>
    {
        public IDictionary<string, string> GetQueryParameters(Context context)
        {
            switch (context)
            {
                case Context.Products:
                    return Models.QueryParameters.QueryParametersCollections.GamesAjaxFiltered;
                case Context.AccountProducts:
                    return Models.QueryParameters.QueryParametersCollections.AccountGetFilteredProducts;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
