using Interfaces.ImageUri;

using GOG.Models;

namespace GOG.Controllers.ImageUri
{
    public class ProductGetImageUriDelegate: IGetImageUriDelegate<Product>
    {
        public string GetImageUri(Product product)
        {
            return product.Image;
        }
    }
}
