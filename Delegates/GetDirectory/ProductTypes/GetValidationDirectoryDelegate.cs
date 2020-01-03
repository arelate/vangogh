using Interfaces.Delegates.GetDirectory;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetMd5DirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetMd5DirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Md5, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}