using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Activities
{
    public static class Context
    {
        public const string Products = "products";
        public const string AccountProducts = "accountProducts";
        public const string Wishlist = "wishlist";
        public const string GameProductData = "gameProductData";
        public const string ApiProducts = "apiProducts";
        public const string GameDetails = "gameDetails";
        public const string Screenshots = "screenshots";
        private const string ImagesSuffix = "Images";
        public const string ProductsImages = Products + ImagesSuffix;
        public const string AccountProductsImages = AccountProducts + ImagesSuffix;
        private const string FilesSuffix = "Files";
        public const string ProductsFiles = Products + FilesSuffix;
        public const string Data = "data";
        public const string Files = "files";
        public const string Directories = "directories";
        public const string Updated = "updated";
    }
}
