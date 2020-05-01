using System.Collections.Generic;

namespace Interfaces.Delegates.Collections
{
    public interface IIntersectDelegate<T>
    {
        IEnumerable<T> Intersect(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection);
    }
}