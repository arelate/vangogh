namespace Interfaces.Settings
{
    public interface IProductsImagesProperty
    {
        bool ProductsImages { get; set; }
    }

    public interface IAccountProductsImagesProperty
    {
        bool AccountProductsImages {get; set; }
    }

    public interface IProductsFilesProperty
    {
        bool ProductsFiles { get; set; }
    }

    public interface ILanguagesProperty
    {
        string[] Languages { get; set; }
    }

    public interface IOperatingSystemsProperty
    {
        string[] OperatingSystems { get; set; }
    }

    public interface IDownloadProperties:
        IProductsImagesProperty,
        IAccountProductsImagesProperty,
        IScreenshotsProperty,
        IProductsFilesProperty,
        ILanguagesProperty,
        IOperatingSystemsProperty
    {
        // ...
    }
}
