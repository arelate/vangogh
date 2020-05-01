using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetAccountProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetAccountProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.AccountProducts, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}