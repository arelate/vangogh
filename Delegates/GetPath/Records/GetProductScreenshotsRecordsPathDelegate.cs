using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetProductScreenshotsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetProductScreenshotsFilenameDelegate))]
        public GetProductScreenshotsRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getProductScreenshotsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductScreenshotsFilenameDelegate)
        {
            // ...
        }
    }
}