using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetProductsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetProductsFilenameDelegate,Delegates")]
        public GetProductsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getProductsFilenameDelegate)
        {
            // ...
        }
    }
}