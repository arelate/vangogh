using Interfaces.Delegates.Values;
using GOG.Models;

namespace GOG.Delegates.Values.Images
{
    public class GetProductImageUriDelegate : IGetValueDelegate<string, Product>
    {
        public string GetValue(Product product)
        {
            return product.Image;
        }
    }
}