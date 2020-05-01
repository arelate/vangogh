using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindStringDelegate : FindDelegate<string>
    {
        [Dependencies(
            typeof(FindAllDelegate<string>))]
        public FindStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate) :
            base(findAllStringDelegate)
        {
            // ...
        }
    }
}