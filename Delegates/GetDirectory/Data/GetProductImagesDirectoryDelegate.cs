using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.Data
{
    public class GetProductImagesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetProductImagesDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.ProductImages, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}