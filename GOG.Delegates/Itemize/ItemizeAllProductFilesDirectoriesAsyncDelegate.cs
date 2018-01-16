using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Directory;

using Interfaces.Status;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private IGetDirectoryAsyncDelegate productFilesDirectoryDelegate;
        private IDirectoryController directoryController;
        private IStatusController statusController;

        public ItemizeAllProductFilesDirectoriesAsyncDelegate(
            IGetDirectoryAsyncDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> ItemizeAllAsync(IStatus status)
        {
            var enumerateProductFilesDirectoriesTask = await statusController.CreateAsync(
                status,
                "Enumerate productFiles directories");

            var directories = new List<string>();

            var productFilesDirectory = await productFilesDirectoryDelegate.GetDirectoryAsync(string.Empty, enumerateProductFilesDirectoriesTask);
            directories.AddRange(directoryController.EnumerateDirectories(productFilesDirectory));

            await statusController.CompleteAsync(enumerateProductFilesDirectoriesTask);

            return directories;
        }
    }
}
