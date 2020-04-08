using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetCookiesFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetCookiesFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate) :
            base(Filenames.Cookies, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}