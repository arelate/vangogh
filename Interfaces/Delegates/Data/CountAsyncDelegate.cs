using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface ICountAsyncDelegate
    {
        Task<long> CountAsync();
    }
}