using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetUpdatedRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetUpdatedFilenameDelegate))]
        public GetUpdatedRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getUpdatedFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}