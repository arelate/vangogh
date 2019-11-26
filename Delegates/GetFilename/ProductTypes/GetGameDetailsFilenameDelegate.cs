using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameDetailsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetGameDetailsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.GameDetails, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}