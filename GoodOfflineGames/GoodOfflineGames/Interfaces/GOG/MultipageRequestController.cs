using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IRequestDelegate<Type>
    {
        Task<IList<Type>> Request(
            string uri, 
            IDictionary<string, string> parameters, 
            IList<Type> filter = null);
    }
}
