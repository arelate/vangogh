using System.IO;

using Interfaces.Destination.Directory;

namespace Controllers.Destination.Directory
{
    public class RelativeDirectoryDelegate : IGetDirectoryDelegate
    {
        private string baseDirectory;
        private IGetDirectoryDelegate[] parentDirectories;

        public RelativeDirectoryDelegate(string baseDirectory, params IGetDirectoryDelegate[] parentDirectories)
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
