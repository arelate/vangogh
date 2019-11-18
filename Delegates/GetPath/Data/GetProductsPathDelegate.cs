using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Data
{
    public class GetProductsPathDelegate : GetPathDelegate
    {
        public GetProductsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getProductsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductsFilenameDelegate)
        {
            // ...
        }
    }
}