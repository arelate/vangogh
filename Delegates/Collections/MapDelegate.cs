using System;
using System.Collections.Generic;

using Interfaces.Delegates.Collections;

namespace Delegates.Collections
{
    public abstract class MapDelegate<T> : IMapDelegate<T>
    {
        public void Map(IEnumerable<T> collection, Action<T> map)
        {
            if (collection == null) return;

            foreach (var item in collection)
                map(item);
        }
    }
}