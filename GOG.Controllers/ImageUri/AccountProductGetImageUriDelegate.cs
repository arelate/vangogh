using Interfaces.ImageUri;

using GOG.Models;

namespace GOG.Controllers.ImageUri
{
    public class AccountProductGetImageUriDelegate: IGetImageUriDelegate<AccountProduct>
    {
        public string GetImageUri(AccountProduct accountProduct)
        {
            return accountProduct.Image;
        }
    }
}