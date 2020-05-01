using System.Threading.Tasks;
using Interfaces.Delegates.Download;
using GOG.Interfaces.Delegates.DownloadProductFile;
using Attributes;
using Delegates.Download;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadProductImageAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        private readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;

        [Dependencies(
            typeof(DownloadFromUriAsyncDelegate))]
        public DownloadProductImageAsyncDelegate(IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate)
        {
            this.downloadFromUriAsyncDelegate = downloadFromUriAsyncDelegate;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination)
        {
            if (downloadFromUriAsyncDelegate != null)
                await downloadFromUriAsyncDelegate?.DownloadFromUriAsync(
                    sourceUri,
                    destination);
        }
    }
}