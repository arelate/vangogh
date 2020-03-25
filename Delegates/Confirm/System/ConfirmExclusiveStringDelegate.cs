using Attributes;

using Interfaces.Delegates.Intersect;


namespace Delegates.Confirm.System
{
    public class ConfirmExclusiveStringDelegate : ConfirmExclusiveDelegate<string>
    {
        [Dependencies(
            "Delegates.Intersect.System.IntersectStringDelegate,Delegates")]
        public ConfirmExclusiveStringDelegate(
            IIntersectDelegate<string> intersectStringDelegate) : 
            base(intersectStringDelegate)
        {
            // ...
        }
    }
}