using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class IntersectStringDelegate : IntersectDelegate<string>
    {
        [Dependencies(
            "Delegates.Collections.System.FindAllStringDelegate,Delegates",
            "Delegates.Collections.System.FindStringDelegate,Delegates")]
        public IntersectStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate,
            IFindDelegate<string> findStringDelegate) :
            base(findAllStringDelegate, findStringDelegate)
        {
            // ...
        }
    }
}