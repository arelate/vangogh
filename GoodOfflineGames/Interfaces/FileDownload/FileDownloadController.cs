using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.TaskStatus;

namespace Interfaces.FileDownload
{
    public interface IDownloadFileFromResponseDelegate
    {
        Task DownloadFileFromResponseAsync(HttpResponseMessage response, string destination, ITaskStatus taskStatus);
    }

    public interface IDownloadFileFromSourceDelegate
    {
        Task DownloadFileFromSourceAsync(
            long id,
            string title,
            string sourceUri, 
            string destination, 
            ITaskStatus taskStatus);
    }

    public interface IFileDownloadController:
        IDownloadFileFromResponseDelegate,
        IDownloadFileFromSourceDelegate
    {
        // ...
    }
}
