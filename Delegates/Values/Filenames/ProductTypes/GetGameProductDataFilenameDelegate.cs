using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.ProductTypes
{
    public class GetGameProductDataFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetGameProductDataFilenameDelegate(IGetValueDelegate<string, string> GetBinFilenameDelegate) :
            base(Models.Filenames.Filenames.GameProductData, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}