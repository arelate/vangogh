using System.Collections.Generic;

using Interfaces.File;

namespace Interfaces.Directory
{
    public interface ICreateDelegate
    {
        void Create(string uri);
    }

    public interface IDeleteDelegate
    {
        void Delete(string uri);
    }

    public interface IEnumerateFilesDelegate
    {
        IEnumerable<string> EnumerateFiles(string uri);
    }

    public interface IEnumerateDirectoriesDelegate
    {
        IEnumerable<string> EnumerateDirectories(string uri);
    }

    public interface IDirectoryController :
        ICreateDelegate,
        IDeleteDelegate,
        IExistsDelegate,
        IEnumerateFilesDelegate,
        IEnumerateDirectoriesDelegate,
        IMoveDelegate
    {
        // ...
    }
}
