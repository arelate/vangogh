using System;
using System.Collections.Generic;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public abstract class CollectionController<T>:
        ICollectionController<T>
    {
        public IEnumerable<T> Collection { get; private set; }

        protected CollectionController(IEnumerable<T> items)
        {
            Collection = items;
        }

        public IEnumerable<T> Reduce(Predicate<T> condition)
        {
            foreach (var item in Collection)
            {
                if (condition(item)) yield return item;
            }
        }

        public T Find(Predicate<T> findContition)
        {
            foreach (var item in Collection)
            {
                if (findContition(item)) return item;
            }

            return default(T);
        }

        public void Map(Predicate<T> action)
        {
            foreach (var item in Collection)
            {
                action(item);
            }
        }
    }
}
