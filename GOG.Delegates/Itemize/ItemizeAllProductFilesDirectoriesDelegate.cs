using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Directory;

using Interfaces.Status;

using Attributes;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IGetDirectoryDelegate productFilesDirectoryDelegate;
        readonly IDirectoryController directoryController;
        readonly IStatusController statusController;

		[Dependencies(
			"Delegates.GetDirectory.ProductTypes.GetProductFilesRootDirectoryDelegate,Delegates",
			"Controllers.Directory.DirectoryController,Controllers",
			"Controllers.Status.StatusController,Controllers")]
        public ItemizeAllProductFilesDirectoriesAsyncDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync(IStatus status)
        {
            var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory(string.Empty);
            foreach (var directory in directoryController.EnumerateDirectories(productFilesDirectory))
                yield return directory;
        }
    }
}
