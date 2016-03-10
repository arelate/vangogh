namespace GOG.Interfaces
{
    public interface ICleanupDelegate
    {
        int Cleanup(string removeToFolder, IPostUpdateDelegate postUpdateDelegate = null);
    }

    public interface ICleanupController: ICleanupDelegate
    {
        // ...
    }
}
