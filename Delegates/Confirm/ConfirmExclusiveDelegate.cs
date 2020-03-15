using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Intersect;
using Interfaces.Delegates.Confirm;

namespace Delegates.Confirm
{
    public class ConfirmExclusiveDelegate<T> :
        IConfirmDelegate<(IEnumerable<T>, IEnumerable<T>)>
    {
        private IIntersectDelegate<T> intersectDelegate;

        public ConfirmExclusiveDelegate(IIntersectDelegate<T> intersectDelegate)
        {
            this.intersectDelegate = intersectDelegate;
        }

        public bool Confirm((IEnumerable<T>, IEnumerable<T>) collections)
        {
            return !intersectDelegate.Intersect(
                collections.Item1,
                collections.Item2).Any();
        }
    }
}