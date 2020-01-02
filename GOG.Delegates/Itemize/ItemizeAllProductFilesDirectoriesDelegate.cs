using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.Logs;

using Attributes;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IGetDirectoryDelegate productFilesDirectoryDelegate;
        readonly IDirectoryController directoryController;
        readonly IActionLogController actionLogController;

		[Dependencies(
			"Delegates.GetDirectory.ProductTypes.GetProductFilesRootDirectoryDelegate,Delegates",
			"Controllers.Directory.DirectoryController,Controllers",
			"Controllers.Logs.ActionLogController,Controllers")]
        public ItemizeAllProductFilesDirectoriesAsyncDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController,
            IActionLogController actionLogController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
            this.actionLogController = actionLogController;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory(string.Empty);
            foreach (var directory in directoryController.EnumerateDirectories(productFilesDirectory))
                yield return directory;
        }
    }
}
