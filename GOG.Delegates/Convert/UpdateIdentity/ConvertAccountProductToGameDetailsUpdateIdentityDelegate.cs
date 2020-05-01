using Interfaces.Delegates.Convert;
using GOG.Models;

namespace GOG.Delegates.Convert.UpdateIdentity
{
    public class ConvertAccountProductToGameDetailsUpdateIdentityDelegate : IConvertDelegate<AccountProduct, string>
    {
        public string Convert(AccountProduct accountProduct)
        {
            return accountProduct.Id.ToString();
        }
    }
}