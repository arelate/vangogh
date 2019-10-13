using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetReportTemplateFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetReportTemplateFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.ReportTemplate, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}