using Interfaces.Delegates.GetDirectory;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductFilesDirectoryDelegate : GetUriDirectoryDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetDirectory.ProductTypes.GetProductFilesRootDirectoryDelegate,Delegates")]
        public GetProductFilesDirectoryDelegate(
            IGetDirectoryDelegate getProductFilesRootDirectoryDelegate) :
            base(getProductFilesRootDirectoryDelegate)
        {
            // ...
        }
    }
}