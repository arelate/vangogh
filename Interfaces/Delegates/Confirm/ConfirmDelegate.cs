using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Confirm
{
    public interface IConfirmDelegate<T>
    {
        bool Confirm(T data);
    }

    public interface IConfirmAsyncDelegate<T>
    {
        Task<bool> ConfirmAsync(T data, IStatus status);
    }
}
