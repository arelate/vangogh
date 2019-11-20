using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetRecycleBinDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetRecycleBinDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.RecycleBin, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}