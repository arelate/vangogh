using GOG.Models;
using Interfaces.Delegates.Confirmations;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmAccountProductUpdatedDelegate : IConfirmDelegate<AccountProduct>
    {
        public bool Confirm(AccountProduct accountProduct)
        {
            return accountProduct.IsNew || accountProduct.Updates > 0;
        }
    }
}