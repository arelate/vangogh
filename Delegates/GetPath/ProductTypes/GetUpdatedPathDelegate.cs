using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetUpdatedPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetUpdatedFilenameDelegate))]
        public GetUpdatedPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getUpdatedFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}