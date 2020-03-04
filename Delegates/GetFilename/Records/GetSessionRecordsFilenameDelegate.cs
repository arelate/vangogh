using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Session, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}