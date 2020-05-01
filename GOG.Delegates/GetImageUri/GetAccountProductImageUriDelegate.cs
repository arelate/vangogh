using GOG.Interfaces.Delegates.GetImageUri;
using GOG.Models;

namespace GOG.Delegates.GetImageUri
{
    public class GetAccountProductImageUriDelegate : IGetImageUriDelegate<AccountProduct>
    {
        public string GetImageUri(AccountProduct accountProduct)
        {
            return accountProduct.Image;
        }
    }
}