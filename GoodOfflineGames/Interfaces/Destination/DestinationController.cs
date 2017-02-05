namespace Interfaces.Destination
{
    public interface IGetDirectoryDelegate
    {
        string GetDirectory(string source = null);
    }

    public interface IGetFilenameDelegate
    {
        string GetFilename(string source = null);
    }

    public interface IDestinationController:
        IGetDirectoryDelegate,
        IGetFilenameDelegate
    {
        // ...
    }
}
