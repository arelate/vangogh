using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.ProductTypes
{
    public class GetWishlistedPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetWishlistedFilenameDelegate))]
        public GetWishlistedPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getWishlistedFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getWishlistedFilenameDelegate)
        {
            // ...
        }
    }
}