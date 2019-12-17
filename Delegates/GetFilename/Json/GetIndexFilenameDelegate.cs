using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetIndexFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetIndexFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.Index, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}