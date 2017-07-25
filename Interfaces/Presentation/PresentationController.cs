using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Presentation
{
    public interface IPresentDelegate<T>
    {
        void Present(T views);
    }

    public interface IPresentAsyncDelegate<T>
    {
        Task PresentAsync(T views);
    }

    public interface IPresentationController<T> :
        IPresentDelegate<T>,
        IPresentAsyncDelegate<T>
    {
        // ...
    }
}
