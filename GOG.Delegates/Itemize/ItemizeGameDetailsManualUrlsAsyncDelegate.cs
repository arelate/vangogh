using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

using Models.Uris;
using Models.Settings;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeGameDetailsManualUrlsAsyncDelegate : IItemizeAsyncDelegate<GameDetails, string>
    {
        private IGetDataAsyncDelegate<Settings> getSettingsDataAsyncDelegate;
        private IDataController<GameDetails> gameDetailsDataController;

        public ItemizeGameDetailsManualUrlsAsyncDelegate(
            IGetDataAsyncDelegate<Settings> getSettingsDataAsyncDelegate,
            IDataController<GameDetails> gameDetailsDataController)
        {
            this.getSettingsDataAsyncDelegate = getSettingsDataAsyncDelegate;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        public async Task<IEnumerable<string>> ItemizeAsync(GameDetails gameDetails, IStatus status)
        {
            var settings = await getSettingsDataAsyncDelegate.GetDataAsync(status);

            if (settings == null ||
                settings.DownloadsLanguages == null ||
                settings.DownloadsOperatingSystems == null)
                throw new System.InvalidOperationException("Cannot enumerate game details without settings (even default).");

            var manualUrls = new List<string>();

            if (gameDetails == null) return manualUrls;

            var gameDetailsDownloadEntries = new List<DownloadEntry>();

            if (gameDetails.LanguageDownloads != null)
                foreach (var download in gameDetails.LanguageDownloads)
                {
                    if (!settings.DownloadsLanguages.Contains(download.Language)) continue;

                    if (settings.DownloadsOperatingSystems.Contains("Windows") && download.Windows != null)
                        gameDetailsDownloadEntries.AddRange(download.Windows);
                    if (settings.DownloadsOperatingSystems.Contains("Mac") && download.Mac != null)
                        gameDetailsDownloadEntries.AddRange(download.Mac);
                    if (settings.DownloadsOperatingSystems.Contains("Linux") && download.Linux != null)
                        gameDetailsDownloadEntries.AddRange(download.Linux);
                }

            if (gameDetails.Extras != null)
                gameDetailsDownloadEntries.AddRange(gameDetails.Extras);

            foreach (var downloadEntry in gameDetailsDownloadEntries)
            {
                var absoluteUri = string.Format(Uris.Paths.ProductFiles.ManualUrlRequestTemplate, downloadEntry.ManualUrl);
                manualUrls.Add(absoluteUri);
            }

            // last but not least - recursively add DLCs
            if (gameDetails.DLCs != null)
                foreach (var dlc in gameDetails.DLCs)
                    manualUrls.AddRange(await ItemizeAsync(dlc, status));

            return manualUrls;
        }

    }
}
