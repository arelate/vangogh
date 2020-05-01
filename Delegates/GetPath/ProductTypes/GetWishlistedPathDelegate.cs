using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetWishlistedPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetWishlistedFilenameDelegate))]
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