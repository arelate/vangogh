using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Status;

using Models.Separators;

namespace Delegates.GetDirectory
{
    public class GetUriDirectoryDelegate : IGetDirectoryDelegate
    {
        readonly IGetDirectoryDelegate baseDirectoryDelegate;

        public GetUriDirectoryDelegate(IGetDirectoryDelegate baseDirectoryDelegate)
        {
            this.baseDirectoryDelegate = baseDirectoryDelegate;
        }

        public string GetDirectory(string source)
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
                baseDirectory = baseDirectoryDelegate.GetDirectory(string.Empty);

            return Path.Combine(baseDirectory, directory);
        }
    }
}
