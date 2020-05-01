using System.IO;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.GetPath;

namespace Delegates.GetPath
{
    public abstract class GetPathDelegate : IGetPathDelegate
    {
        private readonly IGetDirectoryDelegate getDirectoryDelegate;
        private readonly IGetFilenameDelegate getFilenameDelegate;

        public GetPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
        }

        public string GetPath(string directoryInput, string filenameInput)
        {
            return Path.Combine(
                getDirectoryDelegate.GetDirectory(directoryInput),
                getFilenameDelegate.GetFilename(filenameInput));
        }
    }
}