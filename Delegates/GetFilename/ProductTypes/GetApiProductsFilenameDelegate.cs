using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetApiProductsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetApiProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate):
            base(Filenames.ApiProducts, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}