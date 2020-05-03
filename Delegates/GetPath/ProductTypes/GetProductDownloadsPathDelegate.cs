using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductDownloadsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetProductDownloadsFilenameDelegate))]
        public GetProductDownloadsPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getProductDownloadsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductDownloadsFilenameDelegate)
        {
            // ...
        }
    }
}