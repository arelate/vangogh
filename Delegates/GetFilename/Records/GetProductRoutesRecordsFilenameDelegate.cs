using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetProductRoutesRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetProductRoutesRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.ProductRoutes, getBinFilenameDelegate)
        {
            // ...
        }
    }
}