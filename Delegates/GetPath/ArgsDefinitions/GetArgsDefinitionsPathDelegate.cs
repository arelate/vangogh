using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ArgsDefinitions
{
    public class GetArgsDefinitionsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.Root.GetEmptyDirectoryDelegate),
            typeof(Delegates.GetFilename.ArgsDefinitions.GetArgsDefinitionsFilenameDelegate))]
        public GetArgsDefinitionsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate) :
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}