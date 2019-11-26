using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductDownloadsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetProductDownloadsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.ProductDownloads, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}