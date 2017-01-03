namespace Interfaces.Settings
{
    public interface IDirectoriesProperty {
        bool Directories { get; set; }
    }

    public interface IFilesProperty {
        bool Files { get; set; }
    }

    public interface ICleanupProperties:
        IDirectoriesProperty,
        IFilesProperty
    {
        // ...
    }
}
