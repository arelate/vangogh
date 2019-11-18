using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetGameDetailsRecordsPathDelegate : GetPathDelegate
    {
        public GetGameDetailsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getGameDetailsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getGameDetailsFilenameDelegate)
        {
            // ...
        }
    }
}