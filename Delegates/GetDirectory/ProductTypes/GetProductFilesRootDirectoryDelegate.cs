using Interfaces.Delegates.GetDirectory;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductFilesRootDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetProductFilesRootDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.ProductFiles, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}