using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.TaskStatus;

namespace Interfaces.Download
{
    public interface IDownloadFileDelegate
    {
        Task DownloadFileAsync(HttpResponseMessage response, string destination, ITaskStatus taskStatus);
    }

    public interface IDownloadController:
        IDownloadFileDelegate
    {
        // ...
    }
}
