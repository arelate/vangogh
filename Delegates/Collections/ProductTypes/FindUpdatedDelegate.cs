using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;
using Delegates.Collections.ProductTypes;

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