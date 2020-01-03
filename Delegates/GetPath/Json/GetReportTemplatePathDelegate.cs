using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Delegates.GetPath.Json
{
    public class GetReportTemplatePathDelegate: GetPathDelegate
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetDirectory.Root.GetTemplatesDirectoryDelegate,Delegates",
            "Delegates.GetFilename.Json.GetReportTemplateFilenameDelegate,Delegates")]
        public GetReportTemplatePathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}