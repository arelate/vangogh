using System.Threading.Tasks;
using System.Net.Http;

namespace Interfaces.Download
{
    public interface IDownloadFileDelegate
    {
        Task DownloadFileAsync(HttpResponseMessage response, string destination);
    }

    public interface IDownloadController:
        IDownloadFileDelegate
    {
        // ...
    }
}
