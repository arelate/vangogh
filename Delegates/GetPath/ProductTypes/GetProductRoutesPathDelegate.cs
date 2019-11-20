using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductRoutesPathDelegate : GetPathDelegate
    {
        public GetProductRoutesPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getProductRoutesFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductRoutesFilenameDelegate)
        {
            // ...
        }
    }
}