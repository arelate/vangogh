using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetIndexFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetIndexFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.Index, getBinFilenameDelegate)
        {
            // ...
        }
    }
}