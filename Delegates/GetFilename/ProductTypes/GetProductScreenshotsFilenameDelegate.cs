using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate):
            base(Filenames.ProductScreenshots, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}