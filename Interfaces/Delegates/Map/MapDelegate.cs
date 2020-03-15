using System;
using System.Collections.Generic;

namespace Interfaces.Delegates.Map
{
    public interface IMapDelegate<T>
    {
        void Map(IEnumerable<T> collection, Action<T> map);
    }
}