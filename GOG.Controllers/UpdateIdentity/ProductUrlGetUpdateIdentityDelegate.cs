using Interfaces.UpdateIdentity;

using GOG.Models;

namespace GOG.Controllers.UpdateIdentity
{
    public class ProductUrlGetUpdateIdentityDelegate : IGetUpdateIdentityDelegate<Product>
    {
        public string GetUpdateIdentity(Product item)
        {
            return item.Url;
        }
    }
}
