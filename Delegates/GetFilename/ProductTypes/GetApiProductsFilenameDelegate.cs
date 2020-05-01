using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetApiProductsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetApiProductsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.ApiProducts, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}