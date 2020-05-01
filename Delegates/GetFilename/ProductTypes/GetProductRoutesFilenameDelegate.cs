using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductRoutesFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetProductRoutesFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.ProductRoutes, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}