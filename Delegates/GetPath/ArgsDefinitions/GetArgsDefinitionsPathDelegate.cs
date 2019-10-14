using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.ArgsDefinitions
{
    public class GetArgsDefinitionsPathDelegate: GetPathDelegate
    {
        public GetArgsDefinitionsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate):
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}