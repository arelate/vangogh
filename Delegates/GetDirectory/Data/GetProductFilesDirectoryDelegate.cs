using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.Data
{
    public class GetProductFilesDirectoryDelegate : GetUriDirectoryDelegate
    {
        public GetProductFilesDirectoryDelegate(
            IGetDirectoryDelegate getProductFilesRootDirectoryDelegate) :
            base(getProductFilesRootDirectoryDelegate)
        {
            // ...
        }
    }
}