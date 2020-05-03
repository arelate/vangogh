using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetAccountProductsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetAccountProductsFilenameDelegate))]
        public GetAccountProductsPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getAccountProductsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getAccountProductsFilenameDelegate)
        {
            // ...
        }
    }
}