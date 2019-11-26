using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.ProductScreenshots, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}