using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetAccountProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetAccountProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.AccountProducts, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}