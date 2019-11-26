using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetWishlistedFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetWishlistedFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.Wishlisted, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}