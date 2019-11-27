using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Attributes;

namespace Delegates.GetPath.Json
{
    public class GetAppTemplatePathDelegate: GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetTemplatesDirectoryDelegate,Delegates",
            "Delegates.GetFilename.Json.GetAppTemplateFilenameDelegate,Delegates")]
        public GetAppTemplatePathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}