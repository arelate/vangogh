using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Data;
using Interfaces.Enumeration;

using Models.Uris;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsManualUrlEnumerationController : IEnumerateDelegate<string>
    {
        private string[] languages;
        private string[] operatingSystems;
        private IDataController<GameDetails> gameDetailsDataController;

        public GameDetailsManualUrlEnumerationController(
            string[] languages,
            string[] operatingSystems,
            IDataController<GameDetails> gameDetailsDataController)
        {
            this.languages = languages;
            this.operatingSystems = operatingSystems;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        private IList<string> Enumerate(GameDetails gameDetails)
        {
            var gameDetailsManualUrls = new List<string>();

            if (gameDetails == null) return gameDetailsManualUrls;

            var gameDetailsDownloadEntries = new List<DownloadEntry>();

            if (gameDetails.LanguageDownloads != null)
                foreach (var download in gameDetails.LanguageDownloads)
                {
                    if (!languages.Contains(download.Language)) continue;

                    if (operatingSystems.Contains("Windows") && download.Windows != null)
                        gameDetailsDownloadEntries.AddRange(download.Windows);
                    if (operatingSystems.Contains("Mac") && download.Mac != null)
                        gameDetailsDownloadEntries.AddRange(download.Mac);
                    if (operatingSystems.Contains("Linux") && download.Linux != null)
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
