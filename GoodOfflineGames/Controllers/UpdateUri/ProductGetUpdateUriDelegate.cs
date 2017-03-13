using System;

using Interfaces.UpdateUri;

using Models.Uris;
using Models.ActivityParameters;

namespace Controllers.UpdateUri
{
    public class ProductGetUpdateUriDelegate : IGetUpdateUriDelegate<string>
    {
        public string GetUpdateUri(string productParameter)
        {
            switch (productParameter)
            {
                case Parameters.Products:
                    return Uris.Paths.Games.AjaxFiltered;
                case Parameters.AccountProducts:
                    return Uris.Paths.Account.GetFilteredProducts;
                case Parameters.Screenshots: // screenshots use the same page as game product data
                case Parameters.GameProductData:
                    return Uris.Paths.GameProductData.ProductTemplate;
                case Parameters.ApiProducts:
                    return Uris.Paths.Api.ProductTemplate;
                case Parameters.GameDetails:
                    return Uris.Paths.Account.GameDetailsTemplate;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
