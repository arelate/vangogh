using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameProductDataFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetGameProductDataFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.GameProductData, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}