using System.IO;

namespace GOG.Interfaces
{
    #region Stream IO

    public interface IStreamReadableController
    {
        Stream OpenReadable(string uri);
    }

    public interface IStreamWritableController
    {
        Stream OpenWritable(string uri);
    }

    public interface IStreamController :
        IStreamReadableController,
        IStreamWritableController
    {
        // ...
    }

    // TODO: split
    
    public interface IFileController
    {
        bool ExistsFile(string uri);
        long GetSize(string uri);
    }

    // TODO: split

    public interface IDirectoryController
    {
        void CreateDirectory(string uri);
        bool ExistsDirectory(string uri);
    }

    public interface IIOController :
        IStreamController,
        IFileController,
        IDirectoryController
    {
        // ...
    }

    #endregion
}
