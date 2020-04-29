using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindUpdatedDelegate : FindDelegate<long>
    {
        [Dependencies(
            "Delegates.Collections.ProductTypes.FindAllUpdatedDelegate,Delegates")]
        public FindUpdatedDelegate(
            IFindAllDelegate<long> findAllUpdatedDelegate) :
            base(findAllUpdatedDelegate)
        {
            // ...
        }
    }
}