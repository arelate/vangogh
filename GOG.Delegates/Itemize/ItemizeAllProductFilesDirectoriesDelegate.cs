using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;


using Interfaces.Delegates.Activities;

using Attributes;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IGetDirectoryDelegate productFilesDirectoryDelegate;

		[Dependencies(
			"Delegates.GetDirectory.ProductTypes.GetProductFilesRootDirectoryDelegate,Delegates")]
        public ItemizeAllProductFilesDirectoriesAsyncDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory(string.Empty);
            foreach (var directory in Directory.EnumerateDirectories(productFilesDirectory))
                yield return directory;
        }
    }
}
