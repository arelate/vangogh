using System.IO;

using Interfaces.Destination;

namespace Controllers.Destination
{
    public class FixedDirectoryDelegate : IGetDirectoryDelegate
    {
        private string baseDirectory;

        public FixedDirectoryDelegate(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public string GetDirectory(string relativeDirectory = null)
        {
            return Path.Combine(baseDirectory, relativeDirectory);
        }
    }
}
