using Interfaces.Delegates.GetDirectory;
using Attributes;
using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetRecycleBinDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Root.GetDataDirectoryDelegate))]
        public GetRecycleBinDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.RecycleBin, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}