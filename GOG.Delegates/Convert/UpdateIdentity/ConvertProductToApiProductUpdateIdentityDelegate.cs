using Interfaces.Delegates.Convert;
using GOG.Models;

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