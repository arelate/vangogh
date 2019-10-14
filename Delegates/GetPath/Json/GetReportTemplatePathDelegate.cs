using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Json
{
    public class GetReportTemplatePathDelegate: GetPathDelegate
    {
        public GetReportTemplatePathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}