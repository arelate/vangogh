using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Routing
{
    public interface IUpdateRouteAsyncDelegate
    {
        Task UpdateRouteAsync(long id, string title, string source, string destination, IStatus status);
    }

    public interface ITraceRouteAsyncDelegate
    {
        Task<string> TraceRouteAsync(long id, string source);
    }

    public interface ITraceRoutesAsyncDelegate
    {
        Task<IList<string>> TraceRoutesAsync(long id, IEnumerable<string> sources);
    }

    public interface IRoutingController:
        IUpdateRouteAsyncDelegate,
        ITraceRouteAsyncDelegate,
        ITraceRoutesAsyncDelegate
    {
        // ...
    }
}
