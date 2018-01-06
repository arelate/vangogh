using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Directory;

using Interfaces.Status;

namespace Delegates.Itemize
{
    public class ItemizeMultipleProductFilesDirectoriesAsyncDelegate : IItemizeMultipleAsyncDelegate<string>
    {
        private IGetDirectoryDelegate productFilesDirectoryDelegate;
        private IDirectoryController directoryController;
        private IStatusController statusController;

        public ItemizeMultipleProductFilesDirectoriesAsyncDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> ItemizeMulitpleAsync(IStatus status)
        {
            var enumerateProductFilesDirectoriesTask = await statusController.CreateAsync(
                status, 
                "Enumerate productFiles directories");

            var directories = new List<string>();

            await Task.Run(() =>
            {
                var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory();
                directories.AddRange(directoryController.EnumerateDirectories(productFilesDirectory));
            });

            await statusController.CompleteAsync(enumerateProductFilesDirectoriesTask);

            return directories;
        }
    }
}
