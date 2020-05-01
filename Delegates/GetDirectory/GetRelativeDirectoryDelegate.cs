using System.IO;
using Interfaces.Delegates.GetDirectory;

namespace Delegates.GetDirectory
{
    public abstract class GetRelativeDirectoryDelegate : IGetDirectoryDelegate
    {
        private readonly string baseDirectory;
        private readonly IGetDirectoryDelegate[] parentDirectories;

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