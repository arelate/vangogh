using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetAccountProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetAccountProductsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.AccountProducts, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}