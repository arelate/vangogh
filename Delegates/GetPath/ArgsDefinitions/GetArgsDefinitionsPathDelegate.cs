using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Attributes;

namespace Delegates.GetPath.ArgsDefinitions
{
    public class GetArgsDefinitionsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetEmptyDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ArgsDefinitions.GetArgsDefinitionsFilenameDelegate,Delegates")]
        public GetArgsDefinitionsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate) :
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}