using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetAppTemplateFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetAppTemplateFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate) :
            base(Filenames.AppTemplate, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}