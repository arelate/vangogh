using Models.Directories;

namespace Delegates.GetDirectory.Base
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