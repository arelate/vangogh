using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetRecordsDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetRecordsDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Records, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}