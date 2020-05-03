using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.ProductTypes
{
    public class GetProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetProductsFilenameDelegate(IGetValueDelegate<string, string> GetBinFilenameDelegate) :
            base(Models.Filenames.Filenames.Products, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}