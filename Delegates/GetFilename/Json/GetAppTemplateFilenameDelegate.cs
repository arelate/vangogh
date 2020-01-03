using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetAppTemplateFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetAppTemplateFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate) :
            base(Filenames.AppTemplate, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}