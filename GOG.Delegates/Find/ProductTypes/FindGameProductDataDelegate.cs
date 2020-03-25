using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;


using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindGameProductDataDelegate : FindDelegate<GameProductData>
    {
        [Dependencies(
            "GOG.Delegates.Find.ProductTypes.FindAllGameProductDataDelegate,GOG.Delegates")]
        public FindGameProductDataDelegate(
            IFindAllDelegate<GameProductData> findAllGameProductDataDelegate) :
            base(findAllGameProductDataDelegate)
        {
            // ...
        }
    }
}