using Models.Directories;

namespace Delegates.GetDirectory.Base
{
    public class GetTemplatesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetTemplatesDirectoryDelegate() :
            base(Directories.Templates)
        {
            // ...
        }
    }
}