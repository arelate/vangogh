using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductDownloadsDelegate : FindDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(FindAllProductDownloadsDelegate))]
        public FindProductDownloadsDelegate(
            IFindAllDelegate<ProductDownloads> findAllProductDownloadsDelegate) :
            base(findAllProductDownloadsDelegate)
        {
            // ...
        }
    }
}