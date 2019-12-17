using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetProductRoutesRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetProductRoutesFilenameDelegate,Delegates")]
        public GetProductRoutesRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductRoutesFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductRoutesFilenameDelegate)
        {
            // ...
        }
    }
}