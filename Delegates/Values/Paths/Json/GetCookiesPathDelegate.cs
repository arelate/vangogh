using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.Json;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.Json
{
    public class GetCookiesPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetEmptyDirectoryDelegate),
            typeof(GetCookiesFilenameDelegate))]
        public GetCookiesPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getFilenameDelegate) :
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}