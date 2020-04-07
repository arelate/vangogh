using System.Collections.Generic;

using Interfaces.Delegates.Collections;

namespace Delegates.Collections
{
    public abstract class IntersectDelegate<T>: IIntersectDelegate<T>
    {
        private IFindAllDelegate<T> findAllDelegate;
        private IFindDelegate<T> findDelegate;

        public IntersectDelegate(
            IFindAllDelegate<T> findAllDelegate,
            IFindDelegate<T> findDelegate)
        {
            this.findAllDelegate = findAllDelegate;
            this.findDelegate = findDelegate;
        }

        public IEnumerable<T> Intersect(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection)
        {
            return findAllDelegate.FindAll(
                firstCollection,
                firstItem => findDelegate.Find(
                    secondCollection,
                    secondItem => secondItem.Equals(firstItem)) != null);
        }
    }
}

