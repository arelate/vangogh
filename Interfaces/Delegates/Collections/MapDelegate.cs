using System;
using System.Collections.Generic;

namespace Interfaces.Delegates.Collections
{
    public interface IMapDelegate<T>
    {
        void Map(IEnumerable<T> collection, Action<T> map);
    }
}