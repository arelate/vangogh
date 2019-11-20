using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
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