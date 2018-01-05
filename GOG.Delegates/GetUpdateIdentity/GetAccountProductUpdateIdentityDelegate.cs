using GOG.Interfaces.Delegates.GetUpdateIdentity;

using GOG.Models;

namespace GOG.Delegates.GetUpdateIdentity
{
    public class GetAccountProductUpdateIdentityDelegate : IGetUpdateIdentityDelegate<AccountProduct>
    {
        public string GetUpdateIdentity(AccountProduct accountProduct)
        {
            return accountProduct.Id.ToString();
        }
    }
}
