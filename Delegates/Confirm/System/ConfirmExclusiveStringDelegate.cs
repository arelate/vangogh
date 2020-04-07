using Attributes;

using Interfaces.Delegates.Collections;


namespace Delegates.Confirm.System
{
    public class ConfirmExclusiveStringDelegate : ConfirmExclusiveDelegate<string>
    {
        [Dependencies(
            "Delegates.Collections.System.IntersectStringDelegate,Delegates")]
        public ConfirmExclusiveStringDelegate(
            IIntersectDelegate<string> intersectStringDelegate) : 
            base(intersectStringDelegate)
        {
            // ...
        }
    }
}