using Interfaces.UpdateUri;

using GOG.Models;

namespace GOG.Controllers.UpdateUri
{
    public class ProductIdUpdateUriDelegate : IGetUpdateUriDelegate<Product>
    {
        public string GetUpdateUri(Product product)
        {
            return product.Id.ToString();
        }
    }
}
