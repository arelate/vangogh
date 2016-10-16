using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.ProductTypes;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    public class ProductFilesDownloadSourcesController : IDownloadSourcesController
    {
        private IProductTypeStorageController productTypeStorageController;
        private string[] languages;
        private string[] operatingSystems;

        public ProductFilesDownloadSourcesController(
            string[] languages,
            string[] operatingSystems,
            IProductTypeStorageController productTypeStorageController)
        {
            this.languages = languages;
            this.operatingSystems = operatingSystems;

            this.productTypeStorageController = productTypeStorageController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var gameDetailsCollection = await productTypeStorageController.Pull<GameDetails>(ProductTypes.GameDetails);

            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var gameDetails in gameDetailsCollection)
            {
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

                foreach (var extraDownloadEntry in gameDetails.Extras)
                    downloadSources.Add(extraDownloadEntry.ManualUrl);

                //gameDetailsDownloadSources.Add(gameDetails.Id, downloadSources);
                if (!gameDetailsDownloadSources.ContainsKey(gameDetails.Id))
                    gameDetailsDownloadSources.Add(gameDetails.Id, new List<string>());

                foreach (var source in downloadSources)
                    if (!gameDetailsDownloadSources[gameDetails.Id].Contains(source))
                        gameDetailsDownloadSources[gameDetails.Id].Add(source);
            }

            return gameDetailsDownloadSources;
        }
    }
}
