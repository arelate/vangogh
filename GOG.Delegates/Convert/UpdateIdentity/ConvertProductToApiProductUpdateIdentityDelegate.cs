using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Convert.UpdateIdentity
{
    public class ConvertProductToApiProductUpdateIdentityDelegate : IConvertDelegate<Product, string>
    {
        public string Convert(Product product)
        {
            return product.Id.ToString();
        }
    }
}