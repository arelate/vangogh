namespace Interfaces.ActivityDefinitions
{
    public enum Activity
    {
        Authorize,
        Load,
        UpdateData,
        UpdateDownloads,
        Download,
        Validate,
        Repair,
        Cleanup,
        Report
    }

    public enum Context
    {
        None,
        Products,
        AccountProducts,
        Wishlist,
        GameProductData,
        ApiProducts,
        GameDetails,
        Screenshots,
        ProductsImages,
        AccountProductsImages,
        ProductsFiles,
        Data,
        Settings,
        Files,
        Directories,
        Updated
    }
}
