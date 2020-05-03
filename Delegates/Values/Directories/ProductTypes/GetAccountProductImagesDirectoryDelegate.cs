using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Directories.ProductTypes
{
    public class GetAccountProductImagesDirectoryDelegate : GetRelativeDirectoryDelegate
    {
        [Dependencies(
            typeof(Root.GetDataDirectoryDelegate))]
        public GetAccountProductImagesDirectoryDelegate(
            IGetValueDelegate<string,string> getDataDirectoryDelegate) :
            base(Models.Directories.Directories.AccountProductImages, getDataDirectoryDelegate)
        {
            // ...
        }
    }
}