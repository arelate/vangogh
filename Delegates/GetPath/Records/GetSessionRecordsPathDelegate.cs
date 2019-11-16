using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetSessionRecordsPathDelegate: GetPathDelegate
    {
        public GetSessionRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getSessionRecordsFilenameDelegate):
            base(getRecordsDirectoryDelegate, getSessionRecordsFilenameDelegate)
        {
            // ...
        }
    }
}