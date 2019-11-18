using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetProductScreenshotsRecordsPathDelegate : GetPathDelegate
    {
        public GetProductScreenshotsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductScreenshotsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductScreenshotsFilenameDelegate)
        {
            // ...
        }
    }
}