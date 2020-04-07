using System;
using System.Collections.Generic;

namespace Interfaces.Delegates.Collections
{
    public interface IFindDelegate<T>
    {
        T Find(IEnumerable<T> collection, Predicate<T> find);
    }    
}