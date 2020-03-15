using System;
using System.Collections.Generic;

namespace Interfaces.Delegates.Find
{
    public interface IFindDelegate<T>
    {
        T Find(IEnumerable<T> collection, Predicate<T> find);
    }    
}