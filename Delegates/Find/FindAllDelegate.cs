using System;
using System.Collections.Generic;

using Interfaces.Delegates.Find;

namespace Delegates.Find
{
    public abstract class FindAllDelegate<T> : IFindAllDelegate<T>
    {
        public IEnumerable<T> FindAll(IEnumerable<T> collection, Predicate<T> reduce)
        {
            if (collection == null) yield break;

            foreach (var item in collection)
                if (reduce(item)) yield return item;
        }
    }
}