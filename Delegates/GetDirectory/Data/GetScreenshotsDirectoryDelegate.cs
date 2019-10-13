using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.Data
{
    public class GetScreenshotsDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetScreenshotsDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Screenshots, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}