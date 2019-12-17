using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Status;

namespace Interfaces.Delegates.Download
{
    // TODO: Rename to request
    public interface IDownloadFromResponseAsyncDelegate
    {
        Task DownloadFromResponseAsync(HttpResponseMessage response, string destination, IStatus status);
    }
}
