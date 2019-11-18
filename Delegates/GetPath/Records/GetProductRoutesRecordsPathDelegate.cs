using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetProductRoutesRecordsPathDelegate : GetPathDelegate
    {
        public GetProductRoutesRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductRoutesFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductRoutesFilenameDelegate)
        {
            // ...
        }
    }
}