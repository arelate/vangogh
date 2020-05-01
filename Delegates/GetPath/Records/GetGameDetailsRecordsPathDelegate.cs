using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;
using Delegates.GetDirectory.ProductTypes;
using Delegates.GetFilename.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetGameDetailsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetGameDetailsFilenameDelegate))]
        public GetGameDetailsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getGameDetailsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getGameDetailsFilenameDelegate)
        {
            // ...
        }
    }
}