using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.UriRedirection;
using Interfaces.Data;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.Dependencies.Validation
{
    public class ValidationDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IUriRedirectController uriRedirectController;

        private readonly List<string> extensionsWhitelist = new List<string>(4) {
            ".exe", // Windows
            ".bin", // Windows
            ".dmg", // Mac
            ".sh" // Linux
        };

        public ValidationDownloadSourcesController(
            IDataController<ProductDownloads> productDownloadsDataController,
            IUriRedirectController uriRedirectController)
        {
            this.productDownloadsDataController = productDownloadsDataController;
            this.uriRedirectController = uriRedirectController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var validationSources = new Dictionary<long, IList<string>>();

            foreach (var id in productDownloadsDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetById(id);

                foreach (var downloadEntry in productDownloads.Downloads)
                { 
                    // only product files are eligible for validation
                    if (downloadEntry.Type != ProductDownloadTypes.File)
                        continue;

                    // and among product files only executables are eligible for validation
                    if (!extensionsWhitelist.Contains(Path.GetExtension(downloadEntry.SourceUri)))
                        continue;

                    if (!validationSources.ContainsKey(id))
                        validationSources.Add(id, new List<string>());

                    validationSources[id].Add(await uriRedirectController.GetUriRedirect(downloadEntry.SourceUri));
                }
            }

            return validationSources;
        }
    }
}
