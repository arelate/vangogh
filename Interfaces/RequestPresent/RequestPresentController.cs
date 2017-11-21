using Interfaces.Presentation;
using Interfaces.RequestData;

namespace Interfaces.RequestPresent
{
    public interface IRequestPresentController<RequestType, PresentType>:
        IPresentationController<PresentType>,
        IRequestDataController<RequestType>
    {
        // ...
    }
}
