using System.Threading.Tasks;

namespace Interfaces.TaskActivity
{
    public interface IProcessTaskDelegate
    {
        Task ProcessTask();
    }

    public interface ITaskActivityController:
        IProcessTaskDelegate
    {
        // ...
    }
}
