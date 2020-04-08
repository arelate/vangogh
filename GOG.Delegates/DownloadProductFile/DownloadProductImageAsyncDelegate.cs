using System.Threading.Tasks;
using Interfaces.Delegates.Download;
using GOG.Interfaces.Delegates.DownloadProductFile;
using Attributes;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadProductImageAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        private readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;

        [Dependencies(
            "Delegates.Download.DownloadFromUriAsyncDelegate,Delegates")]
        public DownloadProductImageAsyncDelegate(IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate)
        {
            this.downloadFromUriAsyncDelegate = downloadFromUriAsyncDelegate;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination)
        {
            await downloadFromUriAsyncDelegate?.DownloadFromUriAsync(
                sourceUri,
                destination);
        }
    }
}