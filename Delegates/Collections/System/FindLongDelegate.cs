using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindLongDelegate : FindDelegate<long>
    {
        [Dependencies(
            typeof(FindAllLongDelegate))]
        public FindLongDelegate(
            IFindAllDelegate<long> findAllLongDelegate) :
            base(findAllLongDelegate)
        {
            // ...
        }
    }
}