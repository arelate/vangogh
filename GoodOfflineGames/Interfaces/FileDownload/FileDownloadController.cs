using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Status;

namespace Interfaces.FileDownload
{
    public interface IDownloadFileFromResponseDelegate
    {
        Task DownloadFileFromResponseAsync(HttpResponseMessage response, string destination, IStatus status);
    }

    public interface IDownloadFileFromSourceDelegate
    {
        Task DownloadFileFromSourceAsync(
            long id,
            string title,
            string sourceUri, 
            string destination, 
            IStatus status);
    }

    public interface IFileDownloadController:
        IDownloadFileFromResponseDelegate,
        IDownloadFileFromSourceDelegate
    {
        // ...
    }
}
