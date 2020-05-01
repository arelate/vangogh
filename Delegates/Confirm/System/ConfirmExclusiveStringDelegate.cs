using Attributes;
using Interfaces.Delegates.Collections;
using Delegates.Collections.System;

namespace Delegates.Confirm.System
{
    public class ConfirmExclusiveStringDelegate : ConfirmExclusiveDelegate<string>
    {
        [Dependencies(
            typeof(IntersectStringDelegate))]
        public ConfirmExclusiveStringDelegate(
            IIntersectDelegate<string> intersectStringDelegate) :
            base(intersectStringDelegate)
        {
            // ...
        }
    }
}