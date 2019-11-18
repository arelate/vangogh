using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Data
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