using System.Collections.Generic;
using System.Linq;

using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Settings;

using Models.Uris;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsManualUrlEnumerateDelegate : IEnumerateDelegate<GameDetails>
    {
        private ISettingsProperty settingsProperty;
        private IDataController<GameDetails> gameDetailsDataController;

        public GameDetailsManualUrlEnumerateDelegate(
            ISettingsProperty settingsProperty,
            IDataController<GameDetails> gameDetailsDataController)
        {
            this.settingsProperty = settingsProperty;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        public virtual IEnumerable<string> Enumerate(GameDetails gameDetails)
        {
            //var gameDetailsManualUrls = new List<string>();

            if (settingsProperty == null ||
                settingsProperty.Settings == null ||
                settingsProperty.Settings.DownloadsLanguages == null ||
                settingsProperty.Settings.DownloadsOperatingSystems == null)
                //return gameDetailsManualUrls;
                yield break;


            if (gameDetails == null) //return gameDetailsManualUrls;
                yield break;

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
                yield return absoluteUri;
            }

            // last but not least - recursively add DLCs
            if (gameDetails.DLCs != null)
                foreach (var dlc in gameDetails.DLCs)
                    foreach (var dlcManualUrl in Enumerate(dlc))
                        yield return dlcManualUrl;
        }
    }
}
