using System.Threading.Tasks;

namespace Interfaces.Activity
{
    public interface IProcessActivityAsyncDelegate
    {
        Task ProcessActivityAsync();
    }

    public interface IActivity:
        IProcessActivityAsyncDelegate
    {
        // ...
    }
}
