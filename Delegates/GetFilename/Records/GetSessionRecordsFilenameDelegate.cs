using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.Session, getBinFilenameDelegate)
        {
            // ...
        }
    }
}