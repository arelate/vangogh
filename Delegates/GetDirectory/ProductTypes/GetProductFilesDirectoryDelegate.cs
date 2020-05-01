using Interfaces.Delegates.GetDirectory;
using Attributes;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductFilesDirectoryDelegate : GetUriDirectoryDelegate
    {
        [Dependencies(
            typeof(GetProductFilesRootDirectoryDelegate))]
        public GetProductFilesDirectoryDelegate(
            IGetDirectoryDelegate getProductFilesRootDirectoryDelegate) :
            base(getProductFilesRootDirectoryDelegate)
        {
            // ...
        }
    }
}