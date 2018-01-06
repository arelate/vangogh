using System;

using Interfaces.ContextDefinitions;

using Models.Uris;

using GOG.Interfaces.Delegates.GetUpdateUri;

namespace GOG.Delegates.GetUpdateUri
{
    public class GetProductUpdateUriByContextDelegate : IGetUpdateUriDelegate<Context>
    {
        public string GetUpdateUri(Context context)
        {
            switch (context)
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
