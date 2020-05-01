using Interfaces.Delegates.GetDirectory;
using Attributes;
using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductFilesDirectoryDelegate : GetUriDirectoryDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetProductFilesRootDirectoryDelegate))]
        public GetProductFilesDirectoryDelegate(
            IGetDirectoryDelegate getProductFilesRootDirectoryDelegate) :
            base(getProductFilesRootDirectoryDelegate)
        {
            // ...
        }
    }
}