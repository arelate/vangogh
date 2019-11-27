using Interfaces.Delegates.GetDirectory;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetProductImagesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies("Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetProductImagesDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.ProductImages, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}