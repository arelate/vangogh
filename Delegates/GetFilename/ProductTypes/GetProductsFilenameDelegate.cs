using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Products, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}