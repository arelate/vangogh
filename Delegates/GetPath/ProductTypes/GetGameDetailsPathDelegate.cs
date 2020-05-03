using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetGameDetailsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetGameDetailsFilenameDelegate))]
        public GetGameDetailsPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getGameDetailsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getGameDetailsFilenameDelegate)
        {
            // ...
        }
    }
}