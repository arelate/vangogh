using System.IO;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories
{
    public abstract class GetRelativeDirectoryDelegate : IGetValueDelegate<string,string>
    {
        private readonly string baseDirectory;
        private readonly IGetValueDelegate<string,string>[] parentDirectories;

        public GetRelativeDirectoryDelegate(string baseDirectory, params IGetValueDelegate<string,string>[] parentDirectories)
        {
            this.baseDirectory = baseDirectory;
            this.parentDirectories = parentDirectories;
        }

        public string GetValue(string relativeDirectory)
        {
            var currentPath = string.Empty;

            if (parentDirectories != null)
                foreach (var directoryDelegate in parentDirectories)
                    currentPath = Path.Combine(
                        directoryDelegate.GetValue(string.Empty),
                        currentPath);

            if (relativeDirectory == null) relativeDirectory = string.Empty;

            return Path.Combine(currentPath, baseDirectory, relativeDirectory);
        }
    }
}