using System.Threading.Tasks;

namespace GOG.Interfaces.Delegates.DownloadProductFile
{
    public interface IDownloadProductFileAsyncDelegate
    {
        Task DownloadProductFileAsync(
            long id,
            string title,
            string sourceUri,
            string destination);
    }
}
