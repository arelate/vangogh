using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Data
{
    public class GetApiProductsPathDelegate : GetPathDelegate
    {
        public GetApiProductsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getApiProductsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getApiProductsFilenameDelegate)
        {
            // ...
        }
    }
}