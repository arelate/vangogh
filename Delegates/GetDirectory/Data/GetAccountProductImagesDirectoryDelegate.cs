using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.Data
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