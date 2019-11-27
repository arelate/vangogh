using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetReportTemplateFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetReportTemplateFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate) :
            base(Filenames.ReportTemplate, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}