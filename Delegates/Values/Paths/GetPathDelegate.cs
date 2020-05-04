using System.IO;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths
{
    public abstract class GetPathDelegate : IGetValueDelegate<string,(string Directory,string Filename)>
    {
        private readonly IGetValueDelegate<string,string> getDirectoryDelegate;
        private readonly IGetValueDelegate<string, string> getFilenameDelegate;

        public GetPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getFilenameDelegate)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
        }

        public string GetValue((string Directory, string Filename) directoryFilename)
        {
            return Path.Combine(
                getDirectoryDelegate.GetValue(directoryFilename.Directory),
                getFilenameDelegate.GetValue(directoryFilename.Filename));
        }
    }
}