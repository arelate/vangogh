using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Json
{
    public class GetCookiePathDelegate: GetPathDelegate
    {
        public GetCookiePathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}