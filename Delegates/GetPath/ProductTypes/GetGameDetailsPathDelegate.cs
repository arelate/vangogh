using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
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