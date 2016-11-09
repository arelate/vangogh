using System.Collections.Generic;
using System.Linq;

using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    public class ProductFilesDownloadSourcesController : ProductDownloadSourcesController
    {
        private string[] languages;
        private string[] operatingSystems;

        public ProductFilesDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            string[] languages,
            string[] operatingSystems):
            base(
                updatedDataController,
                gameDetailsDataController)
        {
            this.languages = languages;
            this.operatingSystems = operatingSystems;
        }

        internal override List<string> GetDownloadSources(GameDetails gameDetails)
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

            return downloadSources;
        }
    }
}
