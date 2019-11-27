using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.Session, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}