using Attributes;
using Interfaces.Delegates.GetFilename;
using Models.Filenames;

namespace Delegates.GetFilename.Records
{
    public class GetSessionRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetSessionRecordsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Session, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}