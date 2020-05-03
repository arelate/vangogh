namespace Delegates.Values.Directories.Root
{
    public class GetDataDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetDataDirectoryDelegate() :
            base(Models.Directories.Directories.Data)
        {
            // ...
        }
    }
}