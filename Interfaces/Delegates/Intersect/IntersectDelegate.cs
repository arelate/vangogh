using System.Collections.Generic;

namespace Interfaces.Delegates.Intersect
{
    public interface IIntersectDelegate<T>
    {
        IEnumerable<T> Intersect(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection);
    }
}