using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;
using Delegates.GetDirectory.ProductTypes;
using Delegates.GetFilename.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetApiProductsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetApiProductsFilenameDelegate))]
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