using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetApiProductsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
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