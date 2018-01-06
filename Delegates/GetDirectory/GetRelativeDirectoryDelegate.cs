using System.IO;

using Interfaces.Delegates.GetDirectory;

namespace Delegates.GetDirectory
{
    public class GetRelativeDirectoryDelegate : IGetDirectoryDelegate
    {
        private string baseDirectory;
        private IGetDirectoryDelegate[] parentDirectories;

        public GetRelativeDirectoryDelegate(string baseDirectory, params IGetDirectoryDelegate[] parentDirectories)
        {
            this.baseDirectory = baseDirectory;
            this.parentDirectories = parentDirectories;
        }

        public string GetDirectory(string relativeDirectory = null)
        {
            var currentPath = string.Empty;

            if (parentDirectories != null)
                foreach (var directoryDelegate in parentDirectories)
                    currentPath = Path.Combine(directoryDelegate.GetDirectory(), currentPath);

            if (relativeDirectory == null) relativeDirectory = string.Empty;

            return Path.Combine(currentPath, baseDirectory, relativeDirectory);
        }
    }
}
