using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate):
            base(Filenames.Session, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}