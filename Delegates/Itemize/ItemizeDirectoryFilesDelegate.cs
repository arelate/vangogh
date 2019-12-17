using System.Collections.Generic;

using Interfaces.Controllers.Directory;

using Interfaces.Delegates.Itemize;

using Attributes;

namespace Delegates.Itemize
{
    public class ItemizeDirectoryFilesDelegate : IItemizeDelegate<string, string>
    {
        readonly IDirectoryController directoryController;

        [Dependencies(
            "Controllers.Directory.DirectoryController,Controllers")]
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
