using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetWishlistedRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetWishlistedFilenameDelegate))]
        public GetWishlistedRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getWishlistedFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getWishlistedFilenameDelegate)
        {
            // ...
        }
    }
}