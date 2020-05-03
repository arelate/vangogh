using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames;

namespace Delegates.GetPath.Json
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