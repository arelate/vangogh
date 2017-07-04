namespace Models.Activities
{
    public static class BuiltinActivities
    {
        public const string Authorize = "authorize";

        public static class UpdateData
        {
            private const string UpdateDataPrefix = "updateData-";
            public const string Products = UpdateDataPrefix + Context.Products;
            public const string AccountProducts = UpdateDataPrefix + Context.AccountProducts;
            public const string Wishlist = UpdateDataPrefix + Context.Wishlist;
            public const string GameProductData = UpdateDataPrefix + Context.GameProductData;
            public const string ApiProducts = UpdateDataPrefix + Context.ApiProducts;
            public const string GameDetails = UpdateDataPrefix + Context.GameDetails;
            public const string Screenshots = UpdateDataPrefix + Context.Screenshots;
        }

        public static class UpdateDownloads
        {
            private const string UpdateDownloadsPrefix = "updateDownloads-";
            public const string ProductsImages = UpdateDownloadsPrefix + Context.ProductsImages;
            public const string AccountProductsImages = UpdateDownloadsPrefix + Context.AccountProductsImages;
            public const string Screenshots = UpdateDownloadsPrefix + Context.Screenshots;
            public const string ProductsFiles = UpdateDownloadsPrefix + Context.ProductsFiles;
        }

        public static class Download
        {
            private const string DownloadPrefix = "download-";
            public const string ProductsImages = DownloadPrefix + Context.ProductsImages;
            public const string AccountProductsImages = DownloadPrefix + Context.AccountProductsImages;
            public const string Screenshots = DownloadPrefix + Context.Screenshots;
            public const string ProductsFiles = DownloadPrefix + Context.ProductsFiles;
        }

        public static class Validate
        {
            private const string ValidatePrefix = "validate-";
            public const string ProductFiles = ValidatePrefix + Context.ProductsFiles;
            public const string Data = ValidatePrefix + Context.Data;
        }

        public static class Repair
        {
            private const string RepairPrefix = "repair-";
            public const string ProductsFiles = RepairPrefix + Context.ProductsFiles;
        }

        public static class Cleanup
        {
            private const string CleanupPrefix = "cleanup-";
            public const string Files = CleanupPrefix + Context.Files;
            public const string Directories = CleanupPrefix + Context.Directories;
            public const string Updated = CleanupPrefix + Context.Updated;
        }
    }
}
