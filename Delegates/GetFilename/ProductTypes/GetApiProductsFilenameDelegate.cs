using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetApiProductsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetApiProductsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.ApiProducts, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}