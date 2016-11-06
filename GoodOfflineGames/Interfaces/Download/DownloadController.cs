using System.Threading.Tasks;

namespace Interfaces.Download
{
    public interface IDownloadFileDelegate
    {
        Task<string> DownloadFile(string uri, string destination);
    }

    public interface IDownloadController:
        IDownloadFileDelegate
    {
        // ...
    }
}
