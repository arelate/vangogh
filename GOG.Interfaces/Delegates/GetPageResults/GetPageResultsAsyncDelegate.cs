using System.Collections.Generic;
using System.Threading.Tasks;
using GOG.Interfaces.Models;

namespace GOG.Interfaces.Delegates.GetPageResults
{
    public interface IGetPageResultsAsyncDelegate<T> where T : IPageResult
    {
        Task<IList<T>> GetPageResultsAsync();
    }
}