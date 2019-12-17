using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Attributes;

namespace Delegates.GetPath.Json
{
    public class GetCookiesPathDelegate: GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetEmptyDirectoryDelegate,Delegates",
            "Delegates.GetFilename.Json.GetCookiesFilenameDelegate,Delegates")]
        public GetCookiesPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}