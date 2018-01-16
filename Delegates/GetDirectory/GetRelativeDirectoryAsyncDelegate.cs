using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Status;

namespace Delegates.GetDirectory
{
    public class GetRelativeDirectoryAsyncDelegate : IGetDirectoryAsyncDelegate
    {
        private string baseDirectory;
        private IGetDirectoryAsyncDelegate[] parentDirectories;

        public GetRelativeDirectoryAsyncDelegate(string baseDirectory, params IGetDirectoryAsyncDelegate[] parentDirectories)
        {
            this.baseDirectory = baseDirectory;
            this.parentDirectories = parentDirectories;
        }

        public async Task<string> GetDirectoryAsync(string relativeDirectory, IStatus status)
        {
            var currentPath = string.Empty;

            if (parentDirectories != null)
                foreach (var directoryDelegate in parentDirectories)
                    currentPath = Path.Combine(
                        await directoryDelegate.GetDirectoryAsync(string.Empty, status), 
                        currentPath);

            if (relativeDirectory == null) relativeDirectory = string.Empty;

            return Path.Combine(currentPath, baseDirectory, relativeDirectory);
        }
    }
}
