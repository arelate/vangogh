using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindStringDelegate : FindDelegate<string>
    {
        [Dependencies(
            typeof(FindAllStringDelegate))]
        public FindStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate) :
            base(findAllStringDelegate)
        {
            // ...
        }
    }
}