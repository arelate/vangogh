using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetUpdatedFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetUpdatedFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.Updated, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}