namespace Delegates.GetDirectory.Base
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