using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Products, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}