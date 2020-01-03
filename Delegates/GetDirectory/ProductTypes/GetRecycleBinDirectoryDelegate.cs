using Interfaces.Delegates.GetDirectory;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetRecycleBinDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetRecycleBinDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.RecycleBin, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}