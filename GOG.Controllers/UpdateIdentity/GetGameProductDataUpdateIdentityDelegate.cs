using Interfaces.UpdateIdentity;

using GOG.Models;

namespace GOG.Controllers.UpdateIdentity
{
    public class GetGameProductDataUpdateIdentityDelegate : IGetUpdateIdentityDelegate<Product>
    {
        public string GetUpdateIdentity(Product item)
        {
            return item.Url;
        }
    }
}
