using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductDownloadsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetProductDownloadsFilenameDelegate))]
        public GetProductDownloadsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getProductDownloadsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductDownloadsFilenameDelegate)
        {
            // ...
        }
    }
}