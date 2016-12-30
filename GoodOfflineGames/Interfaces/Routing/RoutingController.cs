using System.Threading.Tasks;

namespace Interfaces.Routing
{
    public interface IAddRouteDelegate
    {
        Task AddRoute(string source, string destination);
    }

    public interface IGetDestinationDelegate
    {
        Task<string> GetDestination(string source);
    }

    public interface IRoutingController:
        IGetDestinationDelegate,
        IAddRouteDelegate
    {
        // ...
    }
}
