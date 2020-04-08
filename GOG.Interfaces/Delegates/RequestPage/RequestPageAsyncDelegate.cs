using System.Collections.Generic;
using System.Threading.Tasks;


namespace GOG.Interfaces.Delegates.RequestPage
{
    public interface IRequestPageAsyncDelegate
    {
        Task<string> RequestPageAsync(string uri, IDictionary<string, string> parameters, int page);
    }
}