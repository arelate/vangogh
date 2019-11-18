using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetGameProductDataRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetGameProductDataRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.GameProductData, getBinFilenameDelegate)
        {
            // ...
        }
    }
}