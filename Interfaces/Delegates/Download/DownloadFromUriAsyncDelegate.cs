using System.Threading.Tasks;

namespace Interfaces.Delegates.Download
{
    // TODO: Rename to request
    public interface IDownloadFromUriAsyncDelegate
    {
        Task DownloadFromUriAsync(string uri, string destination);
    }
}
