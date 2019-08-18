using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Controllers.Collection;

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

        public IEnumerable<T> Intersect<T>(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection)
        {
            return Reduce(
                firstCollection,
                firstItem => Find(
                    secondCollection, 
                    secondItem => secondItem.Equals(firstItem)) != null);
        }

        public bool ConfirmExclusive<T>(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection)
        {
            return !Intersect(firstCollection, secondCollection).Any();
        }        
    }
}
