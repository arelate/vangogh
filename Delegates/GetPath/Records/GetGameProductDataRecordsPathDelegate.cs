using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetGameProductDataRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetGameProductDataFilenameDelegate))]
        public GetGameProductDataRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getGameProductDataFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getGameProductDataFilenameDelegate)
        {
            // ...
        }
    }
}