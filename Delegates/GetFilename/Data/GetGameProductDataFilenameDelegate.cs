using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Data
{
    public class GetGameProductDataFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetGameProductDataFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.GameProductData, getBinFilenameDelegate)
        {
            // ...
        }
    }
}