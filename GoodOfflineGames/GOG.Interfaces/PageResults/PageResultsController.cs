using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

using GOG.Models;

namespace GOG.Interfaces.PageResults
{
    public interface IGetPageResults<T> where T: PageResult {
        Task<IList<T>> GetPageResults(IStatus status);
    }

    public interface IPageResultsController<T>:
        IGetPageResults<T> where T: PageResult
    {
        // ...
    }
}
