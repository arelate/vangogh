using System;
using System.Collections.Generic;

using Interfaces.Delegates.GetQueryParameters;

using Interfaces.Models.Entities;

namespace Delegates.GetQueryParameters
{
    public class GetQueryParametersForProductContextDelegate : IGetQueryParametersDelegate<Entity>
    {
        public IDictionary<string, string> GetQueryParameters(Entity context)
        {
            switch (context)
            {
                case Entity.Products:
                    return Models.QueryParameters.QueryParametersCollections.GamesAjaxFiltered;
                case Entity.AccountProducts:
                    return Models.QueryParameters.QueryParametersCollections.AccountGetFilteredProducts;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
