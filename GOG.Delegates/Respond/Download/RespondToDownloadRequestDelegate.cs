using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;

using Models.ProductTypes;
using Models.Separators;

using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Delegates.Respond.Download
{
    public abstract class RespondToDownloadRequestDelegate<Type>: IRespondAsyncDelegate
        where Type:ProductCore
    {
        readonly IDataController<ProductDownloads> productDownloadsDataController;
        readonly IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public RespondToDownloadRequestDelegate(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.productDownloadsDataController = productDownloadsDataController;
            this.downloadProductFileAsyncDelegate = downloadProductFileAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start(
                $"Process updated {typeof(Type)} downloads");

            var emptyProductDownloads = new List<ProductDownloads>();

            await foreach (var productDownloads in productDownloadsDataController.ItemizeAllAsync())
            {
                if (productDownloads == null) continue;

                // await statusController.UpdateProgressAsync(
                //     processDownloadsTask,
                //     ++current,
                //     total,
                //     productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(
                    d =>
                    d.Type == typeof(Type).ToString()).ToArray();

                startDelegate.Start($"Download {typeof(Type)} entries");

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    var sanitizedUri = entry.SourceUri;
                    if (sanitizedUri.Contains(Separators.QueryString))
                        sanitizedUri = sanitizedUri.Substring(0, sanitizedUri.IndexOf(Separators.QueryString, System.StringComparison.Ordinal));

                   setProgressDelegate.SetProgress();

                    await downloadProductFileAsyncDelegate?.DownloadProductFileAsync(
                        productDownloads.Id,
                        productDownloads.Title,
                        sanitizedUri,
                        entry.Destination);

                    startDelegate.Start($"Remove scheduled {typeof(Type)} downloaded entry");

                    productDownloads.Downloads.Remove(entry);
                    await productDownloadsDataController.UpdateAsync(productDownloads);

                    completeDelegate.Complete();
                }

                // if there are no scheduled downloads left - mark file for removal
                if (productDownloads.Downloads.Count == 0)
                    emptyProductDownloads.Add(productDownloads);

                completeDelegate.Complete();
            }

            startDelegate.Start("Clear empty downloads");

            foreach (var productDownload in emptyProductDownloads)
                await productDownloadsDataController.DeleteAsync(productDownload);

            completeDelegate.Complete();

            completeDelegate.Complete();
        }
    }
}
