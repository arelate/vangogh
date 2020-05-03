using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories.ProductTypes
{
    public class GetRecycleBinDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Root.GetDataDirectoryDelegate))]
        public GetRecycleBinDirectoryDelegate(
            IGetValueDelegate<string,string> getDataDirectoryDelegate) :
            base(Models.Directories.Directories.RecycleBin, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}