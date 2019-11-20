using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
{
    public class GetAccountProductsPathDelegate : GetPathDelegate
    {
        public GetAccountProductsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getAccountProductsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getAccountProductsFilenameDelegate)
        {
            // ...
        }
    }
}