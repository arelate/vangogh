using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetAccountProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetAccountProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.AccountProducts, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}