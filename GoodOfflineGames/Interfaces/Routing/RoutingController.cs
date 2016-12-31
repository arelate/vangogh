using System.Threading.Tasks;

namespace Interfaces.Routing
{
    public interface IUpdateRouteDelegate
    {
        Task UpdateRouteAsync(long id, string title, string source, string destination);
    }

    public interface ITraceRouteDelegate
    {
        Task<string> TraceRouteAsync(long id, string source);
    }

    public interface IRoutingController:
        IUpdateRouteDelegate,
        ITraceRouteDelegate
    {
        // ...
    }
}
