using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.Records
{
    public class GetSessionRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetSessionRecordsFilenameDelegate(IGetValueDelegate<string, string> GetBinFilenameDelegate) :
            base(Models.Filenames.Filenames.Session, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}