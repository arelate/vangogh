using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Json
{
    public class GetValidationPathDelegate: GetPathDelegate
    {
        public GetValidationPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}