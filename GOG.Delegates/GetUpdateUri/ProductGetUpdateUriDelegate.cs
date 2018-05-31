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
                    return Uris.Paths.Games.AjaxFiltered;
                case Entity.AccountProducts:
                    return Uris.Paths.Account.GetFilteredProducts;
                case Entity.Screenshots: // screenshots use the same page as game product data
                case Entity.GameProductData:
                    return Uris.Paths.GameProductData.ProductTemplate;
                case Entity.ApiProducts:
                    return Uris.Paths.Api.ProductTemplate;
                case Entity.GameDetails:
                    return Uris.Paths.Account.GameDetailsRequestTemplate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
