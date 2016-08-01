using System.Collections.Generic;

namespace Interfaces.Filtering
{
    public interface IFilterDelegate<Type>
    {
        IEnumerable<Type> Filter(
            IEnumerable<Type> collection, 
            IList<Type> filter);
    }
}
