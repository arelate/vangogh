using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Activity
{
    public interface IProcessActivityAsyncDelegate
    {
        Task ProcessActivityAsync(IStatus status, params string[] parameters);
    }

    public interface IActivity:
        IProcessActivityAsyncDelegate
    {
        // ...
    }
}
