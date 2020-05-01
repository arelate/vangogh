using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetProductDownloadsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetProductDownloadsFilenameDelegate))]
        public GetProductDownloadsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductDownloadsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductDownloadsFilenameDelegate)
        {
            // ...
        }
    }
}