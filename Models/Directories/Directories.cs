using System;
using System.Collections.Generic;

using Interfaces.Models.Entities;

namespace Models.Directories
{
    public static class Directories
    {
<<<<<<< HEAD
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
=======
        public static string Data = "data";
        public static string RecycleBin = "recycleBin";
        public static string Images = "images";
        public static string Reports = "reports";
        public static string Md5 = "md5";
        public static string ProductFiles = "productFiles";
        public static string Screenshots = "screenshots";
        public static string Records = "records";
        public static string Templates = "templates";
    }
>>>>>>> 031a7711257f6ae79974a8ec1a38663d42935d5c

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