using System.Collections.Generic;

namespace Interfaces.Tree
{
    public interface IToListDelegate<T>
    {
        IList<T> ToList(T item);
    }

    public interface ITreeToListController<T>:
        IToListDelegate<T>
    {
        // ...
    }
}
