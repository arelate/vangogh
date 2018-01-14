using Interfaces.Delegates.Confirm;

using GOG.Models;

namespace GOG.Delegates.Confirm
{
    public class ConfirmAccountProductHasUpdatesDelegate : IConfirmDelegate<AccountProduct>
    {
        public bool Confirm(AccountProduct accountProduct)
        {
            return accountProduct.Updates > 0;
        }
    }
}
