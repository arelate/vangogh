using System;
using System.Collections.Generic;

using Interfaces.Collection;

namespace Controllers.Collection
{
    public class CollectionController : ICollectionController
    {
        public T Find<T>(IEnumerable<T> collection, Predicate<T> find)
        {
            foreach (var item in Reduce(collection, find))
                return item;

            return default(T);
        }

        public void Map<T>(IEnumerable<T> collection, Action<T> map)
        {
            if (collection == null) return;

            foreach (var item in collection)
                map(item);
        }

        public IEnumerable<T> Reduce<T>(IEnumerable<T> collection, Predicate<T> reduce)
        {
            if (collection == null) yield break;

            foreach (var item in collection)
                if (reduce(item)) yield return item;
        }
    }
}
