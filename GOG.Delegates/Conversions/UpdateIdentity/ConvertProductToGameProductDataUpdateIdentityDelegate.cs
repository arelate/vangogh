using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Conversions.UpdateIdentity
{
    public class ConvertProductToGameProductDataUpdateIdentityDelegate : IConvertDelegate<Product, string>
    {
        public string Convert(Product item)
        {
            return item.Url;
        }
    }
}