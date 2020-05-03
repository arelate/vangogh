using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductRoutesPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetProductRoutesFilenameDelegate))]
        public GetProductRoutesPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getProductRoutesFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductRoutesFilenameDelegate)
        {
            // ...
        }
    }
}