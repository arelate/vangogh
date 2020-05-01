using Interfaces.Delegates.GetDirectory;
using Attributes;
using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductFilesRootDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.Root.GetDataDirectoryDelegate))]
        public GetProductFilesRootDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.ProductFiles, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}