using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.ProductTypes
{
    public class FindWishlistedDelegate : FindDelegate<long>
    {
        [Dependencies(
            typeof(FindAllWishlistedDelegate))]
        public FindWishlistedDelegate(
            IFindAllDelegate<long> findAllWishlistedDelegate) :
            base(findAllWishlistedDelegate)
        {
            // ...
        }
    }
}