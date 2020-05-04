using Attributes;
using Delegates.Collections;
using GOG.Models;
using Interfaces.Delegates.Collections;

namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindGameProductDataDelegate : FindDelegate<GameProductData>
    {
        [Dependencies(
            typeof(FindAllGameProductDataDelegate))]
        public FindGameProductDataDelegate(
            IFindAllDelegate<GameProductData> findAllGameProductDataDelegate) :
            base(findAllGameProductDataDelegate)
        {
            // ...
        }
    }
}