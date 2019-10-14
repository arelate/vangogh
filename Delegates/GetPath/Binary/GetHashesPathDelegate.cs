using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Binary
{
    public class GetHashesPathDelegate: GetPathDelegate
    {
        public GetHashesPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}