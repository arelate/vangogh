using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IFilterDelegate<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> products);
    }

    public interface IPagedResultFilterDelegate:
        IFilterDelegate<Product>
    {
        // ...
    }
}
