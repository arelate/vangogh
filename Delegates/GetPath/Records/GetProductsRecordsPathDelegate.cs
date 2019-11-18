using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetProductsRecordsPathDelegate : GetPathDelegate
    {
        public GetProductsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductsFilenameDelegate)
        {
            // ...
        }
    }
}