using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetAccountProductsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetAccountProductsFilenameDelegate,Delegates")]
        public GetAccountProductsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getAccountProductsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getAccountProductsFilenameDelegate)
        {
            // ...
        }
    }
}