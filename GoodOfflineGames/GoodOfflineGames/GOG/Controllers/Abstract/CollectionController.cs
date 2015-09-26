using System;
using System.Collections.Generic;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public abstract class CollectionController<T>:
        ICollectionController<T>
    {
        public IList<T> Collection { get; private set; }

        protected CollectionController(IList<T> items)
        {
            Collection = items;
        }

        public bool Contains(T item)
        {
            return Collection.Contains(item);
        }

        public void Add(T item)
        {
            if (item == null) return;
            if (!Contains(item)) Collection.Add(item);
        }

        public void Insert(int index, T item)
        {
            if (item == null) return;
            if (!Contains(item)) Collection.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return Collection.Remove(item);
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

        public IEnumerable<T> Reduce(Predicate<T> condition)
        {
            foreach (var item in Collection)
            {
                if (condition(item)) yield return item;
            }
        }

    }
}
