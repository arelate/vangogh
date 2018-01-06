using System;
using System.Collections.Generic;

using Interfaces.Delegates.GetQueryParameters;

using Interfaces.ContextDefinitions;

namespace Delegates.GetQueryParameters
{
    public class GetQueryParametersForProductContextDelegate : IGetQueryParametersDelegate<Context>
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
