using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductScreenshotsDelegate : FindDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(FindAllProductScreenshotsDelegate))]
        public FindProductScreenshotsDelegate(
            IFindAllDelegate<ProductScreenshots> findAllProductScreenshotsDelegate) :
            base(findAllProductScreenshotsDelegate)
        {
            // ...
        }
    }
}