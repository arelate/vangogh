using System;
using System.Collections.Generic;

using Interfaces.QueryParameters;

using Models.ActivityParameters;

namespace Controllers.QueryParameters
{
    public class ProductParameterGetQueryParametersDelegate : IGetQueryParametersDelegate<string>
    {
        public IDictionary<string, string> GetQueryParameters(string productParameter)
        {
            switch (productParameter)
            {
                case Parameters.Products:
                    return Models.QueryParameters.QueryParameters.GamesAjaxFiltered;
                case Parameters.AccountProducts:
                    return Models.QueryParameters.QueryParameters.AccountGetFilteredProducts;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
