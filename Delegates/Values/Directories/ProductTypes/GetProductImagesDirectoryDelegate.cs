using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories.ProductTypes
{
    public class GetProductImagesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Root.GetDataDirectoryDelegate))]
        public GetProductImagesDirectoryDelegate(
            IGetValueDelegate<string,string> getDataDirectoryDelegate) :
            base(Models.Directories.Directories.ProductImages, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}