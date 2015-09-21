using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IRequestDelegate<T, F>
    {
        Task<IList<T>> Request(
            string uri, 
            IDictionary<string, string> parameters, 
            IList<F> filter = null);
    }

    public interface IMultipageRequestController<T,F>:
        IRequestDelegate<T,F>
    {
        // ...
    }
}
