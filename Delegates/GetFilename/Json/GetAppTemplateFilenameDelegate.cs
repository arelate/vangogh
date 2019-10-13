using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetAppTemplateFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetAppTemplateFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.AppTemplate, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}