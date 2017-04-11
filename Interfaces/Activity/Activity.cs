using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Activity
{
    public interface IProcessActivityDelegate
    {
        Task ProcessActivityAsync(IStatus status);
    }

    public interface IActivity:
        IProcessActivityDelegate
    {
        // ...
    }
}
