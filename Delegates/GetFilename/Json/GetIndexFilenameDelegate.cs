using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetIndexFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetIndexFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.Index, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}