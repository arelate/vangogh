using Interfaces.Delegates.GetDirectory;
using Attributes;
using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetScreenshotsDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.Root.GetDataDirectoryDelegate))]
        public GetScreenshotsDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Screenshots, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}