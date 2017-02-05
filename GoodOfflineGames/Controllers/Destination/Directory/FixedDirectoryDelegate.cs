using System.IO;

using Interfaces.Destination.Directory;

namespace Controllers.Destination.Directory
{
    public class FixedDirectoryDelegate : IGetDirectoryDelegate
    {
        private string baseDirectory;

        public FixedDirectoryDelegate(string baseDirectory, params IGetDirectoryDelegate[] parentDirectories)
        {
            var currentPath = string.Empty;

            if (parentDirectories != null)
                foreach (var directoryDelegate in parentDirectories)
                    currentPath = Path.Combine(directoryDelegate.GetDirectory(), currentPath);

            this.baseDirectory = Path.Combine(currentPath, baseDirectory);
        }

        public string GetDirectory(string relativeDirectory = null)
        {
            if (string.IsNullOrEmpty(relativeDirectory))
                return baseDirectory;

            return Path.Combine(baseDirectory, relativeDirectory);
        }
    }
}
