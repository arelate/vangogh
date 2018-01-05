using GOG.Interfaces.Delegates.GetImageUri;

using GOG.Models;

namespace GOG.Delegates.GetImageUri
{
    public class GetProductImageUriDelegate: IGetImageUriDelegate<Product>
    {
        public string GetImageUri(Product product)
        {
            return product.Image;
        }
    }
}
