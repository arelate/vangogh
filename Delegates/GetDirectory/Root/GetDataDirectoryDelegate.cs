using Models.Directories;

namespace Delegates.GetDirectory.Root
{
    public class GetDataDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetDataDirectoryDelegate() :
            base(Directories.Data)
        {
            // ...
        }
    }
}