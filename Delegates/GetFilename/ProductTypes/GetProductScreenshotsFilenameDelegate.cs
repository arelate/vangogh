using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate):
            base(Filenames.ProductScreenshots, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}