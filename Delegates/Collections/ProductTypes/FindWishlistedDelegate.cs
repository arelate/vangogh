using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindWishlistedDelegate : FindDelegate<long>
    {
        [Dependencies(
            "Delegates.Collections.ProductTypes.FindAllWishlistedDelegate,Delegates")]
        public FindWishlistedDelegate(
            IFindAllDelegate<long> findAllWishlistedDelegate) :
            base(findAllWishlistedDelegate)
        {
            // ...
        }
    }
}