using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetWishlistedFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetWishlistedFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.Wishlisted, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}