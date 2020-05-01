using Attributes;
using Delegates.Collections;
using GOG.Models;
using Interfaces.Delegates.Collections;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindApiProductDelegate : FindDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(FindAllApiProductDelegate))]
        public FindApiProductDelegate(
            IFindAllDelegate<ApiProduct> findAllApiProductDelegate) :
            base(findAllApiProductDelegate)
        {
            // ...
        }
    }
}