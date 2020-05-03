using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.Json;

namespace Delegates.GetPath.Json
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