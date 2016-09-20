namespace Interfaces.GOGUri
{
    public interface IGetDirectoryDelegate
    {
        string GetDirectory(string uri);
    }

    public interface IGetFilenameDelegate
    {
        string GetFilename(string uri);
    }

    public interface IGOGUriController:
        IGetFilenameDelegate,
        IGetDirectoryDelegate
    {
        // ...
    }
}
