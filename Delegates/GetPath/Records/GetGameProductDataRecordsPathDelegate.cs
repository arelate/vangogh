using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetGameProductDataRecordsPathDelegate : GetPathDelegate
    {
        public GetGameProductDataRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getGameProductDataFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getGameProductDataFilenameDelegate)
        {
            // ...
        }
    }
}