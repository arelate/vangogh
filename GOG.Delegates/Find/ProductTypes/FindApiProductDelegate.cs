using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;


using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindApiProductDelegate : FindDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Find.ProductTypes.FindAllApiProductDelegate,GOG.Delegates")]
        public FindApiProductDelegate(
            IFindAllDelegate<ApiProduct> findAllApiProductDelegate) :
            base(findAllApiProductDelegate)
        {
            // ...
        }
    }
}