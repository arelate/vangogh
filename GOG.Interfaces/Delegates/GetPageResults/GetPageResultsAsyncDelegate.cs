using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

using GOG.Interfaces.Models;

namespace GOG.Interfaces.Delegates.GetPageResults
{
    public interface IGetPageResultsAsyncDelegate<T> where T: IPageResult {
        Task<IList<T>> GetPageResultsAsync(IStatus status);
    }
}
