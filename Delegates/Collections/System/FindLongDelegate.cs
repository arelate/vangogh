using Attributes;

using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindLongDelegate: FindDelegate<long>
    {
        [Dependencies(
            "Delegates.Collections.System.FindAllLongDelegate,Delegates")]
        public FindLongDelegate(
            IFindAllDelegate<long> findAllLongDelegate):
            base(findAllLongDelegate)
            {
                // ...
            }
    }
}