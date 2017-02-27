using System;
using System.Collections.Generic;

namespace Interfaces.Collection
{
    public interface IMapDelegate
    {
        void Map<T>(IEnumerable<T> collection, Action<T> map);
    }

    public interface IReduceDelegate
    {
        IEnumerable<T> Reduce<T>(IEnumerable<T> collection, Predicate<T> reduce);
    }

    public interface IFindDelegate
    {
        T Find<T>(IEnumerable<T> collection, Predicate<T> find);
    }

    public interface ICollectionController:
        IMapDelegate,
        IReduceDelegate,
        IFindDelegate
    {
        // ...
    }
}
