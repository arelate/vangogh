using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Settings;

using Models.Uris;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsManualUrlEnumerationController : IEnumerateDelegate<string>
    {
        private ISettingsProperty settingsProperty;
        private IDataController<GameDetails> gameDetailsDataController;

        public GameDetailsManualUrlEnumerationController(
            ISettingsProperty settingsProperty,
            IDataController<GameDetails> gameDetailsDataController)
        {
            this.settingsProperty = settingsProperty;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        private IList<string> Enumerate(GameDetails gameDetails)
        {
            var gameDetailsManualUrls = new List<string>();

            if (settingsProperty == null ||
                settingsProperty.Settings == null ||
                settingsProperty.Settings.DownloadsLanguages == null ||
                settingsProperty.Settings.DownloadsOperatingSystems == null)
                return gameDetailsManualUrls;

            if (gameDetails == null) return gameDetailsManualUrls;

            var gameDetailsDownloadEntries = new List<DownloadEntry>();

            if (gameDetails.LanguageDownloads != null)
                foreach (var download in gameDetails.LanguageDownloads)
                {
                    if (!settingsProperty.Settings.DownloadsLanguages.Contains(download.Language)) continue;

                    if (settingsProperty.Settings.DownloadsOperatingSystems.Contains("Windows") && download.Windows != null)
                        gameDetailsDownloadEntries.AddRange(download.Windows);
                    if (settingsProperty.Settings.DownloadsOperatingSystems.Contains("Mac") && download.Mac != null)
                        gameDetailsDownloadEntries.AddRange(download.Mac);
                    if (settingsProperty.Settings.DownloadsOperatingSystems.Contains("Linux") && download.Linux != null)
                        gameDetailsDownloadEntries.AddRange(download.Linux);
                }

            if (gameDetails.Extras != null)
                gameDetailsDownloadEntries.AddRange(gameDetails.Extras);

            foreach (var downloadEntry in gameDetailsDownloadEntries)
            {
                var absoluteUri = string.Format(Uris.Paths.ProductFiles.FullUriTemplate, downloadEntry.ManualUrl);
                gameDetailsManualUrls.Add(absoluteUri);
            }

            // last but not least - recursively add DLCs
            if (gameDetails.DLCs != null)
                foreach (var dlc in gameDetails.DLCs)
                    gameDetailsManualUrls.AddRange(Enumerate(dlc));

            return gameDetailsManualUrls;
        }

        public async virtual Task<IList<string>> EnumerateAsync(long id)
        {
            var gameDetails = await gameDetailsDataController.GetByIdAsync(id);
            return Enumerate(gameDetails);
        }
    }
}
