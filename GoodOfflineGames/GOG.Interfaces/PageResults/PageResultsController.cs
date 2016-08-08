using System.Collections.Generic;
using System.Threading.Tasks;

namespace GOG.Interfaces.PageResults
{
    public interface IGetPageResults<T> {
        Task<IList<T>> GetPageResults();
    }

    public interface IPageResultsController<T>:
        IGetPageResults<T>
    {
        // ...
    }
}
