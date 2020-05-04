using System.Collections.Generic;
using System.IO;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations
{
    public class ItemizeDirectoryFilesDelegate : IItemizeDelegate<string, string>
    {
        public IEnumerable<string> Itemize(string directory)
        {
            return Directory.EnumerateFiles(directory);
        }
    }
}