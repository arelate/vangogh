using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetWishlistedFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(Delegates.GetFilename.GetBinFilenameDelegate))]
        public GetWishlistedFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Wishlisted, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}