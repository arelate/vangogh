using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetGameProductDataRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetGameProductDataFilenameDelegate))]
        public GetGameProductDataRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getGameProductDataFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getGameProductDataFilenameDelegate)
        {
            // ...
        }
    }
}