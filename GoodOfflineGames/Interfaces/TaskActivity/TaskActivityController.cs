using System.Threading.Tasks;

namespace Interfaces.TaskActivity
{
    public interface IProcessTaskDelegate
    {
        Task ProcessTaskAsync();
    }

    public interface ITaskActivityController:
        IProcessTaskDelegate
    {
        // ...
    }
}
