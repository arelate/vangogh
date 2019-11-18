using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Data
{
    public class GetGameProductDataPathDelegate : GetPathDelegate
    {
        public GetGameProductDataPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getGameProductDataFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getGameProductDataFilenameDelegate)
        {
            // ...
        }
    }
}