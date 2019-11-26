using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetWishlistedRecordsPathDelegate : GetPathDelegate
    {
        public GetWishlistedRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getWishlistedFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getWishlistedFilenameDelegate)
        {
            // ...
        }
    }
}