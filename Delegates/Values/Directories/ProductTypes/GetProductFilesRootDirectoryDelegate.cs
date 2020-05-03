using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories.ProductTypes
{
    public class GetProductFilesRootDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Root.GetDataDirectoryDelegate))]
        public GetProductFilesRootDirectoryDelegate(
            IGetValueDelegate<string,string> getDataDirectoryDelegate) :
            base(Models.Directories.Directories.ProductFiles, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}