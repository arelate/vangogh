using Interfaces.UpdateIdentity;

using GOG.Models;

namespace GOG.Controllers.UpdateIdentity
{
    public class ProductGetUpdateIdentityDelegate : IGetUpdateIdentityDelegate<Product>
    {
        public string GetUpdateIdentity(Product product)
        {
            return product.Id.ToString();
        }
    }
}
