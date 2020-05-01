using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductScreenshotsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetProductScreenshotsFilenameDelegate))]
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