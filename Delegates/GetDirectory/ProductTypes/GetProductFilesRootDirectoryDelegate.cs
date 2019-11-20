using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductFilesRootDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetProductFilesRootDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.ProductFiles, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}