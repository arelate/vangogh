using System.Threading.Tasks;

namespace Interfaces.Delegates.Confirm
{
    public interface IConfirmDelegate<in T>
    {
        bool Confirm(T data);
    }

    public interface IConfirmAsyncDelegate<in T>
    {
        Task<bool> ConfirmAsync(T data);
    }
}