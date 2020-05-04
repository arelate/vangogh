using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.Json
{
    public class GetGameDetailsFilesPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetProductFilesDirectoryDelegate),
            typeof(GetUriFilenameDelegate))]
        public GetGameDetailsFilesPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getFilenameDelegate) :
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}