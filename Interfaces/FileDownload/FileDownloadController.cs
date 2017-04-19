using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Status;

namespace Interfaces.FileDownload
{
    public interface IDownloadFileFromResponseAsyncDelegate
    {
        Task DownloadFileFromResponseAsync(HttpResponseMessage response, string destination, IStatus status);
    }

    public interface IDownloadFileFromSourceAsyncDelegate
    {
        Task DownloadFileFromSourceAsync(
            long id,
            string title,
            string sourceUri, 
            string destination, 
            IStatus status);
    }

    public interface IFileDownloadController:
        IDownloadFileFromResponseAsyncDelegate,
        IDownloadFileFromSourceAsyncDelegate
    {
        // ...
    }
}
