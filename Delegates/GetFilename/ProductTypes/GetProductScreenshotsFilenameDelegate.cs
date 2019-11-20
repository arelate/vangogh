using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.ProductScreenshots, getBinFilenameDelegate)
        {
            // ...
        }
    }
}