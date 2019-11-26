using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetUpdatedRecordsPathDelegate : GetPathDelegate
    {
        public GetUpdatedRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getUpdatedFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}