using System.Collections.Generic;

using Interfaces.Enumeration;
using Interfaces.Directory;

namespace Controllers.Enumeration
{
    public class DirectoryFilesEnumerateDelegate : IEnumerateDelegate<string>
    {
        private IDirectoryController directoryController;

        public DirectoryFilesEnumerateDelegate(
            IDirectoryController directoryController)
        {
            this.directoryController = directoryController;
        }

        public IEnumerable<string> Enumerate(string directory)
        {
            return directoryController.EnumerateFiles(directory);
        }
    }
}
