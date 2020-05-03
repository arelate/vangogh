using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories.ProductTypes
{
    public class GetProductFilesDirectoryDelegate : GetUriDirectoryDelegate
    {
        [Dependencies(
            typeof(GetProductFilesRootDirectoryDelegate))]
        public GetProductFilesDirectoryDelegate(
            IGetValueDelegate<string,string> getProductFilesRootDirectoryDelegate) :
            base(getProductFilesRootDirectoryDelegate)
        {
            // ...
        }
    }
}