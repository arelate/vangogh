namespace Delegates.GetDirectory.Root
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