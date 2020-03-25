using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetProductScreenshotsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetProductScreenshotsFilenameDelegate,Delegates")]
        public GetProductScreenshotsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductScreenshotsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductScreenshotsFilenameDelegate)
        {
            // ...
        }
    }
}