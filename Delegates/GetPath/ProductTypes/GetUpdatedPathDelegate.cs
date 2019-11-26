using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ProductTypes
{
    public class GetUpdatedPathDelegate : GetPathDelegate
    {
        public GetUpdatedPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getUpdatedFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}