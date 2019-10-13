using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetUpdatedFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetUpdatedFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.Updated, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}