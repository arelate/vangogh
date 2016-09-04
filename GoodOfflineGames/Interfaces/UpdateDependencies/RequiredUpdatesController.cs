using System.Threading.Tasks;

namespace Interfaces.UpdateDependencies
{
    public interface IGetRequiredUpdatesDelegate
    {
        Task<long[]> GetRequiredUpdates();
    }

    public interface IRequiredUpdatesController:
        IGetRequiredUpdatesDelegate
    {
        // ...
    }
}
