using Interfaces.UpdateIdentity;

using GOG.Models;

namespace GOG.Controllers.UpdateIdentity
{
    public class GetAccountProductUpdateIdentityDelegate : IGetUpdateIdentityDelegate<AccountProduct>
    {
        public string GetUpdateIdentity(AccountProduct accountProduct)
        {
            return accountProduct.Id.ToString();
        }
    }
}
