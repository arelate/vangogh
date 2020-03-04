using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameProductDataFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetGameProductDataFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate):
            base(Filenames.GameProductData, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}