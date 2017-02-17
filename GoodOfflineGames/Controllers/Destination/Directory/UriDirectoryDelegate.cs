using System;

using Interfaces.Destination.Directory;

using Models.Separators;

namespace Controllers.Destination.Directory
{
    public class UriDirectoryDelegate : IGetDirectoryDelegate
    {
        private string baseDirectory;

        public UriDirectoryDelegate(IGetDirectoryDelegate baseDirectoryDelegate)
        {
            if (baseDirectoryDelegate != null)
                baseDirectory = baseDirectoryDelegate.GetDirectory();
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

            return (string.IsNullOrEmpty(baseDirectory)) ?
                directory :
                System.IO.Path.Combine(baseDirectory, directory);
        }
    }
}
