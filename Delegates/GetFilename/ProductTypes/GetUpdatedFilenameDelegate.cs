using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetUpdatedFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetUpdatedFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.Updated, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}