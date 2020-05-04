using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.Records;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.Records
{
    public class GetSessionRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetSessionRecordsFilenameDelegate))]
        public GetSessionRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getSessionRecordsFilenameDelegate) :
            base(getRecordsDirectoryDelegate, getSessionRecordsFilenameDelegate)
        {
            // ...
        }
    }
}