using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ArgsDefinitions;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.ArgsDefinitions
{
    public class GetArgsDefinitionsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetEmptyDirectoryDelegate),
            typeof(GetArgsDefinitionsFilenameDelegate))]
        public GetArgsDefinitionsPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getFilenameDelegate) :
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}