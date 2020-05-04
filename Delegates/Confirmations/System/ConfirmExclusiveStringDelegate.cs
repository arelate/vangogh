using Attributes;
using Delegates.Collections.System;
using Interfaces.Delegates.Collections;

namespace Delegates.Confirmations.System
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