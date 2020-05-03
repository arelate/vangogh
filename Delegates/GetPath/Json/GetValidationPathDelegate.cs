using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames;

namespace Delegates.GetPath.Json
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