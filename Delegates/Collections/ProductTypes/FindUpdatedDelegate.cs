using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.ProductTypes
{
    public class FindUpdatedDelegate : FindDelegate<long>
    {
        [Dependencies(
            typeof(FindAllUpdatedDelegate))]
        public FindUpdatedDelegate(
            IFindAllDelegate<long> findAllUpdatedDelegate) :
            base(findAllUpdatedDelegate)
        {
            // ...
        }
    }
}