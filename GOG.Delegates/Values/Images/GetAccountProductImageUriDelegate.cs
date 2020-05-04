using Interfaces.Delegates.Values;
using GOG.Models;

namespace GOG.Delegates.Values.Images
{
    public class GetAccountProductImageUriDelegate : IGetValueDelegate<string, AccountProduct>
    {
        public string GetValue(AccountProduct accountProduct)
        {
            return accountProduct.Image;
        }
    }
}