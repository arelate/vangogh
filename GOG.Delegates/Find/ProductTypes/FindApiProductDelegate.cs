using GOG.Models;
using Attributes;
using Delegates.Collections;
using Interfaces.Delegates.Collections;


namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindApiProductDelegate : FindDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Collections.ProductTypes.FindAllApiProductDelegate))]
        public FindApiProductDelegate(
            IFindAllDelegate<ApiProduct> findAllApiProductDelegate) :
            base(findAllApiProductDelegate)
        {
            // ...
        }
    }
}