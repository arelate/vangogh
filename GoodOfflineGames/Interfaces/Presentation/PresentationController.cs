using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Presentation
{
    public interface IPresentDelegate<T>
    {
        void Present(IEnumerable<T> views);
    }

    public interface IPresentAsyncDelegate<T>
    {
        Task PresentAsync(IEnumerable<T> views);
    }

    public interface IPresentationController<T> :
        IPresentDelegate<T>,
        IPresentAsyncDelegate<T>
    {
        // ...
    }
}
