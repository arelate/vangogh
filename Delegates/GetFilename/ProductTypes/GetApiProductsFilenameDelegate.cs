using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetApiProductsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetApiProductsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.ApiProducts, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}