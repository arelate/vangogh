using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetProductDownloadsRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetProductDownloadsRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.ProductDownloads, getBinFilenameDelegate)
        {
            // ...
        }
    }
}