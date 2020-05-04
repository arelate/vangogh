using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Conversions.UpdateIdentity
{
    public class ConvertProductToApiProductUpdateIdentityDelegate : IConvertDelegate<Product, string>
    {
        public string Convert(Product product)
        {
            return product.Id.ToString();
        }
    }
}