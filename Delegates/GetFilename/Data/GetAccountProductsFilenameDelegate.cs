using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Data
{
    public class GetAccountProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetAccountProductsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.AccountProducts, getBinFilenameDelegate)
        {
            // ...
        }
    }
}