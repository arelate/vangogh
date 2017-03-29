using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.Destination.Directory;
using Interfaces.Status;

namespace Controllers.Enumeration
{
    public class ProductFilesDirectoriesEnumerateDelegate : IEnumerateAsyncDelegate
    {
        private IGetDirectoryDelegate productFilesDirectoryDelegate;
        private IDirectoryController directoryController;
        private IStatusController statusController;

        public ProductFilesDirectoriesEnumerateDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async Task<IList<string>> EnumerateAsync(IStatus status)
        {
            var enumerateProductFilesDirectoriesTask = statusController.Create(status, "Enumerate productFiles directories");

            var directories = new List<string>();

            await Task.Run(() =>
            {
                var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory();
                directories.AddRange(directoryController.EnumerateDirectories(productFilesDirectory));
            });

            statusController.Complete(enumerateProductFilesDirectoriesTask);

            return directories;
        }
    }
}
