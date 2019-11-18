using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetGameDetailsRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetGameDetailsRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.GameDetails, getBinFilenameDelegate)
        {
            // ...
        }
    }
}