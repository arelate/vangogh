using System;
using System.Collections.Generic;

using Interfaces.Collection;

namespace Controllers.Collection
{
    public abstract class CollectionController<T>: ICollectionController<T>
    {
        private IList<T> collection { get; set; }

        protected CollectionController(IList<T> items)
        {
            collection = items;
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public void Add(T item)
        {
            if (item == null) return;
            if (!Contains(item)) collection.Add(item);
        }

        //public virtual void UpdateOrAdd(T item)
        //{
        //    throw new NotImplementedException("Cannot update or add for generic type.");
        //}

        public void Insert(int index, T item)
        {
            if (item == null) return;
            if (!Contains(item)) collection.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return collection.Remove(item);
        }

        public T Find(Predicate<T> predicate)
        {
            foreach (var item in collection)
                if (predicate(item)) return item;

            return default(T);
        }

        public void Map(Predicate<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        public IEnumerable<T> Reduce(Predicate<T> condition)
        {
            foreach (var item in collection)
                if (condition(item)) yield return item;
        }

    }
}
