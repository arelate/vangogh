using System.Collections.Generic;

using Interfaces.IO.File;

namespace Interfaces.IO.Directory
{
    public interface ICreateDelegate
    {
        void Create(string uri);
    }

    public interface IGetFilesDelegate
    {
        IEnumerable<string> GetFiles(string uri);
    }

    public interface IDirectoryController :
        ICreateDelegate,
        IExistsDelegate,
        IGetFilesDelegate
    {
        // ...
    }
}
