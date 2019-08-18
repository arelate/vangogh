using System;
using System.Collections.Generic;

namespace Interfaces.Controllers.Collection
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

    public interface IIntersectDelegate
    {
        IEnumerable<T> Intersect<T>(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection);
    }

    public interface IConfirmExclusiveDelegate
    {
        bool ConfirmExclusive<T>(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection);
    }

    public interface ICollectionController:
        IMapDelegate,
        IReduceDelegate,
        IFindDelegate,
        IIntersectDelegate,
        IConfirmExclusiveDelegate        
    {
        // ...
    }
}
