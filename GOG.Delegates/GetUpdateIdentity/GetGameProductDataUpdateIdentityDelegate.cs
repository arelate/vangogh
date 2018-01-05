using GOG.Interfaces.Delegates.GetUpdateIdentity;

using GOG.Models;

namespace GOG.Delegates.GetUpdateIdentity
{
    public class GetGameProductDataUpdateIdentityDelegate : IGetUpdateIdentityDelegate<Product>
    {
        public string GetUpdateIdentity(Product item)
        {
            return item.Url;
        }
    }
}
