using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ArgsDefinitions;

namespace Delegates.GetPath.ArgsDefinitions
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