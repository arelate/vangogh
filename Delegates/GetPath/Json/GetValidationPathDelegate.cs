using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Delegates.GetPath.Json
{
    public class GetValidationPathDelegate: GetPathDelegate
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetDirectory.ProductTypes.GetMd5DirectoryDelegate,Delegates",
            "Delegates.GetFilename.GetValidationFilenameDelegate,Delegates")]
        public GetValidationPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}