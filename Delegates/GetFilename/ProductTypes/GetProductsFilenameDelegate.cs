using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Products, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}