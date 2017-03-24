using System.Collections.Generic;

using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.Destination.Directory;
using Interfaces.TaskStatus;

namespace Controllers.Enumeration
{
    public class ProductFilesDirectoriesEnumerateDelegate : IEnumerateDelegate
    {
        private IGetDirectoryDelegate productFilesDirectoryDelegate;
        private IDirectoryController directoryController;

        public ProductFilesDirectoriesEnumerateDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
        }

        public IEnumerable<string> Enumerate(ITaskStatus taskStatus)
        {
            var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory();
            return directoryController.EnumerateDirectories(productFilesDirectory);
        }
    }
}
