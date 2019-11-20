using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductScreenshotsPathDelegate : GetPathDelegate
    {
        public GetProductScreenshotsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getProductScreenshotsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductScreenshotsFilenameDelegate)
        {
            // ...
        }
    }
}