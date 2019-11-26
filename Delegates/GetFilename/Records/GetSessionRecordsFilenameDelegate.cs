using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.Session, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}