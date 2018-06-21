using System;
using System.Collections.Generic;

using Interfaces.Models.Entities;

namespace Models.Filenames
{
    public static class Filenames
    {
        public static IDictionary<Entity, string> Base = new Dictionary<Entity, string>{
            { Entity.Index, "index" },
            { Entity.AccountProducts, "accountProducts" },
            { Entity.Products, "products" },
            { Entity.GameDetails, "gameDetails" },
            { Entity.GameProductData, "gameProductData" },
            { Entity.ProductScreenshots, "productScreenshots" },
            { Entity.ProductDownloads, "productDownloads" },
            { Entity.ProductRoutes, "productRoutes" },
            { Entity.ApiProducts, "apiProducts" },
            { Entity.ValidationResults, "validationResults" },
            { Entity.Activity, "activity" }
        };
    }
}
