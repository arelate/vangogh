using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetProductRoutesRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetProductRoutesFilenameDelegate))]
        public GetProductRoutesRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getProductRoutesFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductRoutesFilenameDelegate)
        {
            // ...
        }
    }
}