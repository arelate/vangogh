using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.ProductScreenshots, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}