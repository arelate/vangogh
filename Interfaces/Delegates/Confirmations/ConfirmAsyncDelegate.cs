using System.Threading.Tasks;

namespace Interfaces.Delegates.Confirmations
{
    public interface IConfirmAsyncDelegate<in T>
    {
        Task<bool> ConfirmAsync(T data);
    }
}