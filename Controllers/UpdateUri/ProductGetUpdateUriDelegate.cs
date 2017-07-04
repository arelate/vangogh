using System;

using Interfaces.UpdateUri;

using Models.Uris;
using Models.Activities;

namespace Controllers.UpdateUri
{
    public class ProductParameterUpdateUriDelegate : IGetUpdateUriDelegate<string>
    {
        public string GetUpdateUri(string productParameter)
        {
            switch (productParameter)
            {
                case Context.Products:
                    return Uris.Paths.Games.AjaxFiltered;
                case Context.AccountProducts:
                    return Uris.Paths.Account.GetFilteredProducts;
                case Context.Screenshots: // screenshots use the same page as game product data
                case Context.GameProductData:
                    return Uris.Paths.GameProductData.ProductTemplate;
                case Context.ApiProducts:
                    return Uris.Paths.Api.ProductTemplate;
                case Context.GameDetails:
                    return Uris.Paths.Account.GameDetailsRequestTemplate;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
