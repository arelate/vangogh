using System;
using System.Collections.Generic;

using Interfaces.QueryParameters;

using Models.FlightPlan;

namespace Controllers.QueryParameters
{
    public class ProductParameterGetQueryParametersDelegate : IGetQueryParametersDelegate<string>
    {
        public IDictionary<string, string> GetQueryParameters(string productParameter)
        {
            switch (productParameter)
            {
                case Parameters.Products:
                    return Models.QueryParameters.QueryParametersCollections.GamesAjaxFiltered;
                case Parameters.AccountProducts:
                    return Models.QueryParameters.QueryParametersCollections.AccountGetFilteredProducts;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
