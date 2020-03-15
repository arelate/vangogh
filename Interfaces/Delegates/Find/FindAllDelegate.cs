using System;
using System.Collections.Generic;

namespace Interfaces.Delegates.Find
{
    public interface IFindAllDelegate<T>
    {
        IEnumerable<T> FindAll(IEnumerable<T> collection, Predicate<T> condition);
    }
}