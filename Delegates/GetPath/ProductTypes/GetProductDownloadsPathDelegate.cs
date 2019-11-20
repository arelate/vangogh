using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductDownloadsPathDelegate : GetPathDelegate
    {
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