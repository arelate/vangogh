using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories.ProductTypes
{
    public class GetScreenshotsDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Root.GetDataDirectoryDelegate))]
        public GetScreenshotsDirectoryDelegate(
            IGetValueDelegate<string,string> getDataDirectoryDelegate) :
            base(Models.Directories.Directories.Screenshots, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}