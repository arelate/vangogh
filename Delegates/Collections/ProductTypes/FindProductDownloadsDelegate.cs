using Attributes;

using Interfaces.Delegates.Collections;

using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductDownloadsDelegate : FindDelegate<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Collections.ProductTypes.FindAllProductDownloadsDelegate,Delegates")]
        public FindProductDownloadsDelegate(
            IFindAllDelegate<ProductDownloads> findAllProductDownloadsDelegate) :
            base(findAllProductDownloadsDelegate)
        {
            // ...
        }
    }
}