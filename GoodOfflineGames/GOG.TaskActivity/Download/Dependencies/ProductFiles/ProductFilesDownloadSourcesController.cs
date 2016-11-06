using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    public class ProductFilesDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IDataController<long> updatedDataController;
        private string[] languages;
        private string[] operatingSystems;

        public ProductFilesDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            string[] languages,
            string[] operatingSystems)
        {
            this.languages = languages;
            this.operatingSystems = operatingSystems;

            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var gameDetails = await gameDetailsDataController.GetById(id);

                var downloadSources = new List<string>();

                foreach (var languageDownload in gameDetails.LanguageDownloads)
                {
                    if (!languages.Contains(languageDownload.Language)) continue;

                    if (languageDownload.Windows != null &&
                        operatingSystems.Contains("Windows"))
                    {
                        foreach (var windowsDownloadEntry in languageDownload.Windows)
                            downloadSources.Add(windowsDownloadEntry.ManualUrl);
                    }

                    if (languageDownload.Mac != null &&
                        operatingSystems.Contains("Mac"))
                    {
                        foreach (var macDownloadEntry in languageDownload.Mac)
                            downloadSources.Add(macDownloadEntry.ManualUrl);
                    }

                    if (languageDownload.Linux != null &&
                        operatingSystems.Contains("Linux"))
                    {
                        foreach (var linuxDownloadEntry in languageDownload.Linux)
                            downloadSources.Add(linuxDownloadEntry.ManualUrl);
                    }
                }

                if (!gameDetailsDownloadSources.ContainsKey(id))
                    gameDetailsDownloadSources.Add(id, new List<string>());

                foreach (var source in downloadSources)
                    if (!gameDetailsDownloadSources[gameDetails.Id].Contains(source))
                        gameDetailsDownloadSources[gameDetails.Id].Add(source);
            }

            return gameDetailsDownloadSources;
        }
    }
}
