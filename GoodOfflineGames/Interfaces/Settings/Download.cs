namespace Interfaces.Settings
{
    public interface IProductImagesProperty
    {
        bool ProductImages { get; set; }
    }

    public interface IAccountProductsImagesProperty
    {
        bool AccountProductsImages {get; set; }
    }

    public interface IProductFilesProperty
    {
        bool ProductFiles { get; set; }
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
        IProductImagesProperty,
        IAccountProductsImagesProperty,
        IScreenshotsProperty,
        IProductFilesProperty,
        ILanguagesProperty,
        IOperatingSystemsProperty
    {
        // ...
    }
}
