using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindLongDelegate : FindDelegate<long>
    {
        [Dependencies(
            typeof(FindAllDelegate<long>))]
        public FindLongDelegate(
            IFindAllDelegate<long> findAllLongDelegate) :
            base(findAllLongDelegate)
        {
            // ...
        }
    }
}