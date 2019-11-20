using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetProductsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.Products, getBinFilenameDelegate)
        {
            // ...
        }
    }
}