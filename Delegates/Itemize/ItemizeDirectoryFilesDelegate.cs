using System.Collections.Generic;

using Interfaces.Controllers.Directory;

using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize
{
    public class ItemizeDirectoryFilesDelegate : IItemizeDelegate<string, string>
    {
        private IDirectoryController directoryController;

        public ItemizeDirectoryFilesDelegate(
            IDirectoryController directoryController)
        {
            this.directoryController = directoryController;
        }

        public IEnumerable<string> Itemize(string directory)
        {
            return directoryController.EnumerateFiles(directory);
        }
    }
}
