using Interfaces.Delegates.Data;

namespace Interfaces.Routing
{
    public interface IRoutingController<Type> :
        IUpdateAsyncDelegate<Type>,
        IGetDataAsyncDelegate<string, (long Id, string Source)>
    {
        // ...
    }
}