using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
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