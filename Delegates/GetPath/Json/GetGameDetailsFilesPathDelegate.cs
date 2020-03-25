using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.Json
{
    public class GetGameDetailsFilesPathDelegate: GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetProductFilesDirectoryDelegate,Delegates",
            "Delegates.GetFilename.GetUriFilenameDelegate,Delegates")]
        public GetGameDetailsFilesPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}