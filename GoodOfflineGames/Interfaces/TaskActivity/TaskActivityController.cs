using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace Interfaces.Activity
{
    public interface IProcessActivityDelegate
    {
        Task ProcessActivityAsync(ITaskStatus taskStatus);
    }

    public interface IActivity:
        IProcessActivityDelegate
    {
        // ...
    }
}
