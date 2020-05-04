using System.Collections.Generic;
using System.IO;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeAllProductFilesDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private readonly IGetValueDelegate<string,string> productFilesDirectoryDelegate;

        [Dependencies(
            typeof(GetProductFilesRootDirectoryDelegate))]
        public ItemizeAllProductFilesDirectoriesAsyncDelegate(
            IGetValueDelegate<string,string> productFilesDirectoryDelegate)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            var productFilesDirectory = productFilesDirectoryDelegate.GetValue(string.Empty);
            foreach (var directory in Directory.EnumerateDirectories(productFilesDirectory))
                yield return directory;
        }
    }
}