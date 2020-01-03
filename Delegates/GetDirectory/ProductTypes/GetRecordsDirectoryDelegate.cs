using Interfaces.Delegates.GetDirectory;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetRecordsDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetRecordsDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Records, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}