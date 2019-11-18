using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetApiProductRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetApiProductRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.ApiProducts, getBinFilenameDelegate)
        {
            // ...
        }
    }
}