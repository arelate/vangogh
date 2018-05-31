using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Status;

namespace Delegates.GetDirectory
{
    public class GetRelativeDirectoryDelegate : IGetDirectoryDelegate
    {
        readonly string baseDirectory;
        readonly IGetDirectoryDelegate[] parentDirectories;

        public GetRelativeDirectoryDelegate(string baseDirectory, params IGetDirectoryDelegate[] parentDirectories)
        {
            this.baseDirectory = baseDirectory;
            this.parentDirectories = parentDirectories;
        }

        public string GetDirectory(string relativeDirectory)
        {
            var currentPath = string.Empty;

            if (parentDirectories != null)
                foreach (var directoryDelegate in parentDirectories)
                    currentPath = Path.Combine(
                        directoryDelegate.GetDirectory(string.Empty), 
                        currentPath);

            if (relativeDirectory == null) relativeDirectory = string.Empty;

            return Path.Combine(currentPath, baseDirectory, relativeDirectory);
        }
    }
}
