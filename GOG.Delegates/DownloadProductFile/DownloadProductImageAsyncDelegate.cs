using System.Threading.Tasks;

using Interfaces.Status;

using Interfaces.Delegates.Download;

using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadProductImageAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        private IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;

        public DownloadProductImageAsyncDelegate(IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate)
        {
            this.downloadFromUriAsyncDelegate = downloadFromUriAsyncDelegate;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination, IStatus status)
        {
            await downloadFromUriAsyncDelegate?.DownloadFromUriAsync(
                sourceUri,
                destination,
                status);
        }
    }
}
