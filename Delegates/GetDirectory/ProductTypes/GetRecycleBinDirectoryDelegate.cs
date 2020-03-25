using Interfaces.Delegates.GetDirectory;


using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetRecycleBinDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetRecycleBinDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.RecycleBin, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}