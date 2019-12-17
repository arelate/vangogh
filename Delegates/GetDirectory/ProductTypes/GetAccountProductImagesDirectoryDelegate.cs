using Interfaces.Delegates.GetDirectory;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetAccountProductImagesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies("Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetAccountProductImagesDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.AccountProductImages, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}