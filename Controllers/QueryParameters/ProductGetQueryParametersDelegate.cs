using System;
using System.Collections.Generic;

using Interfaces.QueryParameters;

using Models.Activities;

namespace Controllers.QueryParameters
{
    public class ProductParameterGetQueryParametersDelegate : IGetQueryParametersDelegate<string>
    {
        public IDictionary<string, string> GetQueryParameters(string productParameter)
        {
            switch (productParameter)
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
