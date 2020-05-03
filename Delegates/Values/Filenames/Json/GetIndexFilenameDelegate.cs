using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.Json
{
    public class GetIndexFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetIndexFilenameDelegate(IGetValueDelegate<string, string> getBinFilenameDelegate) :
            base(Models.Filenames.Filenames.Index, getBinFilenameDelegate)
        {
            // ...
        }
    }
}