using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetApiProductsRecordsPathDelegate : GetPathDelegate
    {
        public GetApiProductsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getApiProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getApiProductsFilenameDelegate)
        {
            // ...
        }
    }
}