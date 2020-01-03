using System.Threading.Tasks;

using Interfaces.Delegates.Download;
using Interfaces.Models.Dependencies;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadProductImageAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;

        [Dependencies(
            DependencyContext.Default,
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
