using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetProductScreenshotsRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetProductScreenshotsRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.ProductScreenshots, getBinFilenameDelegate)
        {
            // ...
        }
    }
}