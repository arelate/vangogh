using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetGameProductDataPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetGameProductDataFilenameDelegate))]
        public GetGameProductDataPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getGameProductDataFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getGameProductDataFilenameDelegate)
        {
            // ...
        }
    }
}