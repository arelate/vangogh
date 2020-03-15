using Attributes;

using Interfaces.Delegates.Intersect;
using Interfaces.Models.Dependencies;

namespace Delegates.Confirm.System
{
    public class ConfirmExclusiveStringDelegate : ConfirmExclusiveDelegate<string>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Intersect.System.IntersectStringDelegate,Delegates")]
        public ConfirmExclusiveStringDelegate(
            IIntersectDelegate<string> intersectStringDelegate) : 
            base(intersectStringDelegate)
        {
            // ...
        }
    }
}