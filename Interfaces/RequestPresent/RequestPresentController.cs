using Interfaces.Presentation;
using Interfaces.RequestData;

namespace Interfaces.RequestPresent
{
    public interface IRequestPresentController<T>:
        IPresentationController<T[]>,
        IRequestDataController<T>
    {
        // ...
    }
}
