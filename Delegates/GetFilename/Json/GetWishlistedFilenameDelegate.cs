using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetWishlistedFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetWishlistedFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.Wishlisted, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}