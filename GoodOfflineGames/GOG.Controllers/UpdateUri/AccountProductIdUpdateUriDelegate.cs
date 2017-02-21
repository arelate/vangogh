using Interfaces.UpdateUri;

using GOG.Models;

namespace GOG.Controllers.UpdateUri
{
    public class AccountProductIdUpdateUriDelegate : IGetUpdateUriDelegate<AccountProduct>
    {
        public string GetUpdateUri(AccountProduct accountProduct)
        {
            return accountProduct.Id.ToString();
        }
    }
}
