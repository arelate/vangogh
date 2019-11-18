using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetAccountProductsRecordsPathDelegate : GetPathDelegate
    {
        public GetAccountProductsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getAccountProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getAccountProductsFilenameDelegate)
        {
            // ...
        }
    }
}