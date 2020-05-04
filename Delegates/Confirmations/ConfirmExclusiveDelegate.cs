using System.Collections.Generic;
using System.Linq;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Confirmations;

namespace Delegates.Confirmations
{
    public abstract class ConfirmExclusiveDelegate<T> :
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