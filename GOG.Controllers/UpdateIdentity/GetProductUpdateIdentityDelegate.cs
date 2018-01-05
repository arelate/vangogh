using Interfaces.UpdateIdentity;

using GOG.Models;

namespace GOG.Controllers.UpdateIdentity
{
    public class GetProductUpdateIdentityDelegate : IGetUpdateIdentityDelegate<Product>
    {
        public string GetUpdateIdentity(Product product)
        {
            return product.Id.ToString();
        }
    }
}
