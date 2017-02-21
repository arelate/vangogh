using Interfaces.UpdateUri;

using GOG.Models;

namespace GOG.Controllers.UpdateUri
{
    public class ProductUrlUpdateUriDelegate : IGetUpdateUriDelegate<Product>
    {
        public string GetUpdateUri(Product item)
        {
            return item.Url;
        }
    }
}
