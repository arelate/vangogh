using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Json
{
    public class GetCookiesFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetCookiesFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate):
            base(Filenames.Cookies, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}