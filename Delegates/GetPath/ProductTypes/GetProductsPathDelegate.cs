using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetProductsFilenameDelegate))]
        public GetProductsPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getProductsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductsFilenameDelegate)
        {
            // ...
        }
    }
}