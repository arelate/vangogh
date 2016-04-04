namespace GOG.Interfaces
{
    public interface ICleanupDelegate
    {
        int Cleanup(string removeToFolder, IPostUpdateDelegate postUpdateDelegate = null);
    }

    public interface ICleanupValidationFileDelegate
    {
        bool CleanupValidationFile(string localFile, string removeToFolder);
    }

    public interface ICleanupController :
        ICleanupDelegate,
        ICleanupValidationFileDelegate
    {
        // ...
    }
}
