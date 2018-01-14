using Interfaces.Delegates.Confirm;

using GOG.Models;

namespace GOG.Delegates.Confirm
{
    public class ConfirmAccountProductIsNewDelegate : IConfirmDelegate<AccountProduct>
    {
        public bool Confirm(AccountProduct accountProduct)
        {
            return accountProduct.IsNew;
        }
    }
}
