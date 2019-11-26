using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
{
    public class GetWishlistedPathDelegate : GetPathDelegate
    {
        public GetWishlistedPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getWishlistedFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getWishlistedFilenameDelegate)
        {
            // ...
        }
    }
}