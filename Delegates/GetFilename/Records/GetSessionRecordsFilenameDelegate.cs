using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(Delegates.GetFilename.GetBinFilenameDelegate))]
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Session, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}