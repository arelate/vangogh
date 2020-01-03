using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductScreenshotsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetProductScreenshotsFilenameDelegate,Delegates")]
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