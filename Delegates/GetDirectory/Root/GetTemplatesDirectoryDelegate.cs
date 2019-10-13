using Models.Directories;

namespace Delegates.GetDirectory.Root
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