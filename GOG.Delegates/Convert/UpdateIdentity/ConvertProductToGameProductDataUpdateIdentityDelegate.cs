using Interfaces.Delegates.Convert;

using GOG.Models;

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
