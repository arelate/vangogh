using System.Threading.Tasks;
using System.Net.Http;

namespace Interfaces.Delegates.Download
{
    // TODO: Rename to request
    public interface IDownloadFromResponseAsyncDelegate
    {
        Task DownloadFromResponseAsync(HttpResponseMessage response, string destination);
    }
}
