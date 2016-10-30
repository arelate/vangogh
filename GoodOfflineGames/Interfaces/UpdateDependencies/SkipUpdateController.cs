using System.Threading.Tasks;

namespace Interfaces.UpdateDependencies
{
    public interface ISkipUpdateDelegate
    {
        Task<bool> SkipUpdate(long id);
    }

    public interface ISkipUpdateController:
        ISkipUpdateDelegate
    {
        // ...
    }
}
