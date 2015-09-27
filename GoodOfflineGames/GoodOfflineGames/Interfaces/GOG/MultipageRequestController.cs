using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IRequestDelegate<ReturnType, FilterType>
    {
        Task<IList<ReturnType>> Request(
            string uri, 
            IDictionary<string, string> parameters, 
            IList<FilterType> filter = null);
    }
}
