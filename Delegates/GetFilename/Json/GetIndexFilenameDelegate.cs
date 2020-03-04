using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetIndexFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetIndexFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate):
            base(Filenames.Index, getBinFilenameDelegate)
        {
            // ...
        }
    }
}