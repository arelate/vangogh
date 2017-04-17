using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

using GOG.Interfaces.Models;

namespace GOG.Interfaces.PageResults
{
    public interface IGetPageResults<T> where T: IPageResult {
        Task<IList<T>> GetPageResults(IStatus status);
    }

    public interface IPageResultsController<T>:
        IGetPageResults<T> where T: IPageResult
    {
        // ...
    }
}
