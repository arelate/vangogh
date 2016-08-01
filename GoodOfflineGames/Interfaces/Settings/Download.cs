namespace Interfaces.Settings
{
    public interface IImagesProperty
    {
        bool Images { get; set; }
    }

    public interface IScreenshotsProperty
    {
        bool Screenshots { get; set; }
    }

    public interface IFilesProperty
    {
        bool Files { get; set; }
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
        IImagesProperty,
        IScreenshotsProperty,
        IFilesProperty,
        ILanguagesProperty,
        IOperatingSystemsProperty
    {
        // ...
    }
}
