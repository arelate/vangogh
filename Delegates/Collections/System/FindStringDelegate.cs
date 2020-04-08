using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindStringDelegate : FindDelegate<string>
    {
        [Dependencies(
            "Delegates.Collections.System.FindAllStringDelegate,Delegates")]
        public FindStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate) :
            base(findAllStringDelegate)
        {
            // ...
        }
    }
}