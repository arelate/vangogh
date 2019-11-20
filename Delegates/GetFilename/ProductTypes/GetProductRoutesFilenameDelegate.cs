using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductRoutesFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetProductRoutesFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.ProductRoutes, getBinFilenameDelegate)
        {
            // ...
        }
    }
}