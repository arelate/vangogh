using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Data
{
    public class GetProductDownloadsPathDelegate : GetPathDelegate
    {
        public GetProductDownloadsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getProductDownloadsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductDownloadsFilenameDelegate)
        {
            // ...
        }
    }
}