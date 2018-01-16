using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.GetDirectory
{
    public interface IGetDirectoryAsyncDelegate
    {
        Task<string> GetDirectoryAsync(string input, IStatus status);
    }
}
