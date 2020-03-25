using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetApiProductsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetApiProductsFilenameDelegate,Delegates")]
        public GetApiProductsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getApiProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getApiProductsFilenameDelegate)
        {
            // ...
        }
    }
}