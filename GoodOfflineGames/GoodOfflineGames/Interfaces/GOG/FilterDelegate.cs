using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IFilterDelegate<Type>
    {
        IEnumerable<Type> Filter(
            IEnumerable<Type> collection, 
            IList<Type> filter);
    }
}
