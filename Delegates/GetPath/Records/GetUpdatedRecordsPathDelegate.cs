using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetUpdatedRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetUpdatedFilenameDelegate))]
        public GetUpdatedRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getUpdatedFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}