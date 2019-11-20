using Interfaces.Delegates.GetDirectory;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetReportDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        public GetReportDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Reports, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}