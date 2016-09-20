namespace Interfaces.Destination
{
    public interface IGetDirectoryDelegate
    {
        string GetDirectory(string source);
    }

    public interface IGetFilenameDelegate
    {
        string GetFilename(string source);
    }

    public interface IDestinationController:
        IGetDirectoryDelegate,
        IGetFilenameDelegate
    {
        // ...
    }
}
