using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class IntersectStringDelegate : IntersectDelegate<string>
    {
        [Dependencies(
            typeof(FindAllDelegate<string>),
            typeof(FindStringDelegate))]
        public IntersectStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate,
            IFindDelegate<string> findStringDelegate) :
            base(findAllStringDelegate, findStringDelegate)
        {
            // ...
        }
    }
}