using System.Threading.Tasks;

namespace Interfaces.Delegates.Confirm
{
    public interface IConfirmDelegate<T>
    {
        bool Confirm(T data);
    }

    public interface IConfirmAsyncDelegate<T>
    {
        Task<bool> ConfirmAsync(T data);
    }
}