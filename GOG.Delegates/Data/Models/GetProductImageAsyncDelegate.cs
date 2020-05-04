using System.Threading.Tasks;
using Interfaces.Delegates.Download;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Download;
using GOG.Models;

namespace GOG.Delegates.Data.Models
{
    public class GetProductImageAsyncDelegate : IGetDataAsyncDelegate<string, ProductFileDownloadManifest>
    {
        private readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;

        [Dependencies(
            typeof(DownloadFromUriAsyncDelegate))]
        public GetProductImageAsyncDelegate(IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate)
        {
            this.downloadFromUriAsyncDelegate = downloadFromUriAsyncDelegate;
        }

        public async Task<string> GetDataAsync(ProductFileDownloadManifest productFileDownloadManifest)
        {
            if (downloadFromUriAsyncDelegate != null)
                await downloadFromUriAsyncDelegate?.DownloadFromUriAsync(
                    productFileDownloadManifest.Source,
                    productFileDownloadManifest.Destination);

            return productFileDownloadManifest.Destination;
        }
    }
}