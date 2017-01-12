using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace GOG.Interfaces.PageResults
{
    public interface IGetPageResults<T> {
        Task<IList<T>> GetPageResults(ITaskStatus taskStatus);
    }

    public interface IPageResultsController<T>:
        IGetPageResults<T>
    {
        // ...
    }
}
