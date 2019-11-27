using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.ProductScreenshots, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}