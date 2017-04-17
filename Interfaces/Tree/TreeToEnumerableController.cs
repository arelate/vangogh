using System.Collections.Generic;

namespace Interfaces.Tree
{
    public interface IToEnumerableDelegate<T>
    {
        IEnumerable<T> ToEnumerable(T item);
    }

    public interface IGetChildren<T>
    {
        IEnumerable<T> GetChildren(T item);
    }

    public interface ITreeToEnumerableController<T>:
        IToEnumerableDelegate<T>,
        IGetChildren<T>
    {
        // ...
    }
}
