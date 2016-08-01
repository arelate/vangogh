using System.Collections.Generic;

namespace Interfaces.IO.Directory
{
    public interface ICreateDirectoryDelegate
    {
        void CreateDirectory(string uri);
    }

    public interface IDirectoryExistsDelegate
    {
        bool DirectoryExists(string uri);
    }

    public interface IEnumerateFilesDelegate
    {
        IEnumerable<string> EnumerateFiles(string uri);
    }

    public interface IDirectoryController :
        ICreateDirectoryDelegate,
        IDirectoryExistsDelegate,
        IEnumerateFilesDelegate
    {
        // ...
    }
}
