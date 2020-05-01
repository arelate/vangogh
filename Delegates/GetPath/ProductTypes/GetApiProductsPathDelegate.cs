using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetApiProductsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetApiProductsFilenameDelegate))]
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