using System.Collections.Generic;

using Interfaces.IO.Directory;

namespace Controllers.IO.Directory
{
    public class DirectoryController: IDirectoryController
    {
        public bool Exists(string uri)
        {
            return System.IO.Directory.Exists(uri);
        }

        public void Create(string uri)
        {
            System.IO.Directory.CreateDirectory(uri);
        }

        public IEnumerable<string> GetFiles(string uri)
        {
            return System.IO.Directory.EnumerateFiles(uri);
        }
    }
}
