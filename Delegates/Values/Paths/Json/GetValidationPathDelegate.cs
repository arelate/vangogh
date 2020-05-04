using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.Json
{
    public class GetValidationPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetMd5DirectoryDelegate),
            typeof(GetValidationFilenameDelegate))]
        public GetValidationPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getFilenameDelegate) :
            base(getDirectoryDelegate, getFilenameDelegate)
        {
            // ...
        }
    }
}