using System.Collections.Generic;
using System.IO;

using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize
{
    public class ItemizeDirectoryFilesDelegate : IItemizeDelegate<string, string>
    {
        public IEnumerable<string> Itemize(string directory)
        {
            return Directory.EnumerateFiles(directory);
        }
    }
}
