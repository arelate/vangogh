using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetAccountProductsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetAccountProductsFilenameDelegate))]
        public GetAccountProductsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getAccountProductsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getAccountProductsFilenameDelegate)
        {
            // ...
        }
    }
}