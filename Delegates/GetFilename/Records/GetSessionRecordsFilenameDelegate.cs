using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetSessionRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Session, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}