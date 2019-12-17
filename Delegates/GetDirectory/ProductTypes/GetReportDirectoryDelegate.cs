using Interfaces.Delegates.GetDirectory;

using Attributes;

using Models.Directories;

namespace Delegates.GetDirectory.ProductTypes
{
    public class GetReportDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies("Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates")]
        public GetReportDirectoryDelegate(
            IGetDirectoryDelegate getDataDirectoryDelegate) :
            base(Directories.Reports, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}