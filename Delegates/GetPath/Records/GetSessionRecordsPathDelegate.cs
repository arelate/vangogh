using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;
using Delegates.GetFilename.Records;

namespace Delegates.GetPath.Records
{
    public class GetSessionRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(GetSessionRecordsFilenameDelegate))]
        public GetSessionRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getSessionRecordsFilenameDelegate) :
            base(getRecordsDirectoryDelegate, getSessionRecordsFilenameDelegate)
        {
            // ...
        }
    }
}