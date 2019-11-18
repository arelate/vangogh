using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Data
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