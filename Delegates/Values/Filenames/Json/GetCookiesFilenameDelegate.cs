using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.Json
{
    public class GetCookiesFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetJsonFilenameDelegate))]
        public GetCookiesFilenameDelegate(IGetValueDelegate<string, string> getJsonFilenameDelegate) :
            base(Models.Filenames.Filenames.Cookies, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}