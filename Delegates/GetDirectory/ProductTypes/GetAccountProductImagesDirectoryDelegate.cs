using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetAccountProductImagesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetAccountProductImagesDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.AccountProductImages, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}