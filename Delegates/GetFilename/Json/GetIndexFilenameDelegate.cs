using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetIndexFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(Delegates.GetFilename.GetBinFilenameDelegate))]
        public GetIndexFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.Index, getBinFilenameDelegate)
        {
            // ...
        }
    }
}