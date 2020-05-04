using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Convert.UpdateIdentity
{
    public class ConvertProductToGameProductDataUpdateIdentityDelegate : IConvertDelegate<Product, string>
    {
        public string Convert(Product item)
        {
            return item.Url;
        }
    }
}