using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;
using Delegates.GetDirectory.ProductTypes;
using Delegates.GetFilename.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetProductRoutesRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetProductRoutesFilenameDelegate))]
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