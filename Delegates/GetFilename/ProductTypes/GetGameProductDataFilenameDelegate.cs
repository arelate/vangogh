using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameProductDataFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetGameProductDataFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.GameProductData, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}