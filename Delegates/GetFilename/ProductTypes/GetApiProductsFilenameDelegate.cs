using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetApiProductsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetApiProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate):
            base(Filenames.ApiProducts, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}