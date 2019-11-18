using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Data
{
    public class GetGameDetailsPathDelegate : GetPathDelegate
    {
        public GetGameDetailsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getGameDetailsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getGameDetailsFilenameDelegate)
        {
            // ...
        }
    }
}