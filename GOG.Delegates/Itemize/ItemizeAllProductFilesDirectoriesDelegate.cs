using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;
using Interfaces.Models.Dependencies;

using Interfaces.Controllers.Logs;

using Attributes;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IGetDirectoryDelegate productFilesDirectoryDelegate;
        readonly IActionLogController actionLogController;

		[Dependencies(
            DependencyContext.Default,
			"Delegates.GetDirectory.ProductTypes.GetProductFilesRootDirectoryDelegate,Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public ItemizeAllProductFilesDirectoriesAsyncDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IActionLogController actionLogController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.actionLogController = actionLogController;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory(string.Empty);
            foreach (var directory in Directory.EnumerateDirectories(productFilesDirectory))
                yield return directory;
        }
    }
}
