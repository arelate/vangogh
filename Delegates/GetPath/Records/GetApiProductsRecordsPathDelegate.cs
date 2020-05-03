using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetApiProductsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetApiProductsFilenameDelegate))]
        public GetApiProductsRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getApiProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getApiProductsFilenameDelegate)
        {
            // ...
        }
    }
}