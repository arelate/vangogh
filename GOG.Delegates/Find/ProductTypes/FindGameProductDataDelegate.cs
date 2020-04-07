using GOG.Models;

using Attributes;
using Delegates.Collections;
using Interfaces.Delegates.Collections;


namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindGameProductDataDelegate : FindDelegate<GameProductData>
    {
        [Dependencies(
            "GOG.Delegates.Collections.ProductTypes.FindAllGameProductDataDelegate,GOG.Delegates")]
        public FindGameProductDataDelegate(
            IFindAllDelegate<GameProductData> findAllGameProductDataDelegate) :
            base(findAllGameProductDataDelegate)
        {
            // ...
        }
    }
}