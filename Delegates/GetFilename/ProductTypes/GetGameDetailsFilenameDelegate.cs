using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameDetailsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetGameDetailsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.GameDetails, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}