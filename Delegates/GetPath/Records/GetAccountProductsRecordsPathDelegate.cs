using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetAccountProductsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetAccountProductsFilenameDelegate))]
        public GetAccountProductsRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getAccountProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getAccountProductsFilenameDelegate)
        {
            // ...
        }
    }
}