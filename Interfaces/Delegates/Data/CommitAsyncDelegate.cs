using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface ICommitAsyncDelegate
    {
        Task CommitAsync();
    }
}