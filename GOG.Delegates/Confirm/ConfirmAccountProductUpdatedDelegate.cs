using Interfaces.Delegates.Confirm;

using GOG.Models;

namespace GOG.Delegates.Confirm
{
    public class ConfirmAccountProductUpdatedDelegate : IConfirmDelegate<AccountProduct>
    {
        public bool Confirm(AccountProduct accountProduct)
        {
            return accountProduct.IsNew || accountProduct.Updates > 0;
        }
    }
}
