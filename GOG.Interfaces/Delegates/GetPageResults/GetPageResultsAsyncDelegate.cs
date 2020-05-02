using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Models.PageResult;

namespace GOG.Interfaces.Delegates.GetPageResults
{
    public interface IGetPageResultsAsyncDelegate<T> where T : IPageResult
    {
        Task<IList<T>> GetPageResultsAsync();
    }
}