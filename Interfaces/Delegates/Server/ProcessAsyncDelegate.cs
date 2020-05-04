using System.Threading.Tasks;
using System.Collections.Generic;

namespace Interfaces.Delegates.Server
{
    public interface IProcessAsyncDelegate
    {
        Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters);
    }
}