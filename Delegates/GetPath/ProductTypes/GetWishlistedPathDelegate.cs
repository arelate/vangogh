using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
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