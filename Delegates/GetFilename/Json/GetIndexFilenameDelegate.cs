using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetIndexFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetIndexFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.Index, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}