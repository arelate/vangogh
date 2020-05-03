using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetWishlistedRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetWishlistedFilenameDelegate))]
        public GetWishlistedRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getWishlistedFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getWishlistedFilenameDelegate)
        {
            // ...
        }
    }
}