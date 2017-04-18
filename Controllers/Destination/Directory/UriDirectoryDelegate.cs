using System;
using System.IO;

using Interfaces.Destination.Directory;

using Models.Separators;

namespace Controllers.Destination.Directory
{
    public class UriDirectoryDelegate : IGetDirectoryDelegate
    {
        private IGetDirectoryDelegate baseDirectoryDelegate;


        public UriDirectoryDelegate(IGetDirectoryDelegate baseDirectoryDelegate)
        {
            this.baseDirectoryDelegate = baseDirectoryDelegate;
        }

        public string GetDirectory(string source = null)
        {
            var directory = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var uriParts = source.Split(
                    new string[] { Separators.UriPart },
                    StringSplitOptions.RemoveEmptyEntries);

                directory = uriParts.Length >= 2 ?
                    uriParts[uriParts.Length - 2] :
                    source;
            }

            var baseDirectory = string.Empty;

            if (baseDirectoryDelegate != null)
                baseDirectory = baseDirectoryDelegate.GetDirectory();

            return Path.Combine(baseDirectory, directory);
        }
    }
}
