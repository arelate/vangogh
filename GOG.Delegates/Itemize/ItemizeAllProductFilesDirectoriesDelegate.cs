using System.Collections.Generic;
using System.IO;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;
using Attributes;
using Delegates.GetDirectory.ProductTypes;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private readonly IGetDirectoryDelegate productFilesDirectoryDelegate;

        [Dependencies(
            typeof(GetProductFilesRootDirectoryDelegate))]
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