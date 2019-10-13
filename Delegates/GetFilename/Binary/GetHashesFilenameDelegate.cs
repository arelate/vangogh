using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetHashesFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetHashesFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.Hashes, getBinFilenameDelegate)
        {
            // ...
        }
    }
}