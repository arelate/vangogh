using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Status;

using Models.Separators;

namespace Delegates.GetDirectory
{
    public class GetUriDirectoryAsyncDelegate : IGetDirectoryAsyncDelegate
    {
        private IGetDirectoryAsyncDelegate baseDirectoryDelegate;

        public GetUriDirectoryAsyncDelegate(IGetDirectoryAsyncDelegate baseDirectoryDelegate)
        {
            this.baseDirectoryDelegate = baseDirectoryDelegate;
        }

        public async Task<string> GetDirectoryAsync(string source, IStatus status)
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
                baseDirectory = await baseDirectoryDelegate.GetDirectoryAsync(string.Empty, status);

            return Path.Combine(baseDirectory, directory);
        }
    }
}
