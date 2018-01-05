using GOG.Interfaces.Delegates.GetUpdateIdentity;

using GOG.Models;

namespace GOG.Delegates.GetUpdateIdentity
{
    public class GetProductUpdateIdentityDelegate : IGetUpdateIdentityDelegate<Product>
    {
        public string GetUpdateIdentity(Product product)
        {
            return product.Id.ToString();
        }
    }
}
