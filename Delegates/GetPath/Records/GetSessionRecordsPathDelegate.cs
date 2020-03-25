using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetSessionRecordsPathDelegate: GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate,Delegates",
            "Delegates.GetFilename.Binary.GetSessionRecordsFilenameDelegate,Delegates")]
        public GetSessionRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getSessionRecordsFilenameDelegate):
            base(getRecordsDirectoryDelegate, getSessionRecordsFilenameDelegate)
        {
            // ...
        }
    }
}