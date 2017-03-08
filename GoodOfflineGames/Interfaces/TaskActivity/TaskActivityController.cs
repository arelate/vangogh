using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace Interfaces.TaskActivity
{
    public interface IProcessTaskDelegate
    {
        Task ProcessTaskAsync(ITaskStatus taskStatus);
    }

    public interface ITaskActivityController:
        IProcessTaskDelegate
    {
        // ...
    }
}
