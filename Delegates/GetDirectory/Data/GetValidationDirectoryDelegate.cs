using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.Data
{
    public class GetMd5DirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetMd5DirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Md5, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}