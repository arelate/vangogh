namespace Delegates.Values.Directories.Root
{
    public class GetEmptyDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetEmptyDirectoryDelegate() :
            base(string.Empty)
        {
            // ...
        }
    }
}