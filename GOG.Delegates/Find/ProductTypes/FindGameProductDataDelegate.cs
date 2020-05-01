using GOG.Models;
using Attributes;
using Delegates.Collections;
using Interfaces.Delegates.Collections;


namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindGameProductDataDelegate : FindDelegate<GameProductData>
    {
        [Dependencies(
            typeof(GOG.Delegates.Collections.ProductTypes.FindAllGameProductDataDelegate))]
        public FindGameProductDataDelegate(
            IFindAllDelegate<GameProductData> findAllGameProductDataDelegate) :
            base(findAllGameProductDataDelegate)
        {
            // ...
        }
    }
}