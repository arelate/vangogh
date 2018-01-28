using System;
using System.Collections.Generic;

using Interfaces.Models.Entities;

namespace Models.Directories
{
    public static class Directories
    {
        public static IDictionary<Entity, string> Data = new Dictionary<Entity, string>
        {
            { Entity.Products, "products" },
            { Entity.AccountProducts, "accountProducts" },
            { Entity.GameDetails, "gameDetails" },
            { Entity.GameProductData, "gameProductData" },
            { Entity.ApiProducts, "apiProducts" },
            { Entity.ProductDownloads, "productDownloads" },
            { Entity.ProductRoutes, "productRoutes" },
            { Entity.Screenshots, "screenshots" },
            { Entity.ValidationResults, "validationResults" }
        };

        public static IDictionary<Entity, string> Base = new Dictionary<Entity, string>
        {
            { Entity.Data, "data" },
            { Entity.RecycleBin, "recycleBin" },
            { Entity.ProductImages, "productsImages" },
            { Entity.AccountProductImages, "accountProductsImages" },
            { Entity.Reports, "reports" },
            { Entity.Md5, "md5" },
            { Entity.ProductFiles, "productFiles"},
            { Entity.Records, "records" }
        };
    }
}