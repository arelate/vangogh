using System;

using Interfaces.Models.Entities;

using Models.Uris;

using GOG.Interfaces.Delegates.GetUpdateUri;

namespace GOG.Delegates.GetUpdateUri
{
    public class GetProductUpdateUriByContextDelegate : IGetUpdateUriDelegate<Entity>
    {
        public string GetUpdateUri(Entity context)
        {
            switch (context)
            {
                case Entity.Products:
                    return Uris.Endpoints.Games.AjaxFiltered;
                case Entity.AccountProducts:
                    return Uris.Endpoints.Account.GetFilteredProducts;
                case Entity.Screenshots: // screenshots use the same page as game product data
                case Entity.GameProductData:
                    return Uris.Endpoints.GameProductData.ProductTemplate;
                case Entity.ApiProducts:
                    return Uris.Endpoints.Api.ProductTemplate;
                case Entity.GameDetails:
                    return Uris.Endpoints.Account.GameDetailsRequestTemplate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
