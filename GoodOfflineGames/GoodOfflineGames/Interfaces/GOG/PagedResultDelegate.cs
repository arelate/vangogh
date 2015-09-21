using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IFilterDelegate<SourceType, FilterType>
    {
        IEnumerable<SourceType> Filter(
            IEnumerable<SourceType> collection, 
            IList<FilterType> filter);
    }
}
