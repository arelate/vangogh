using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.UriRedirection;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.Dependencies.Validation
{
    public class ValidationDownloadSourcesController : IDownloadSourcesController
    {
        //private IProductTypeStorageController productTypeStorageController;
        private IUriRedirectController uriRedirectController;

        private readonly List<string> extensionsWhitelist = new List<string>(4) {
            ".exe", // Windows
            ".bin", // Windows
            ".dmg", // Mac
            ".sh" // Linux
        };

        public ValidationDownloadSourcesController(
            //IProductTypeStorageController productTypeStorageController,
            IUriRedirectController uriRedirectController)
        {
            //this.productTypeStorageController = productTypeStorageController;
            this.uriRedirectController = uriRedirectController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var scheduledDownloads = new List<ScheduledDownload>(); // await productTypeStorageController.Pull<ScheduledDownload>(ProductTypes.ScheduledDownload);
            var validationSources = new Dictionary<long, IList<string>>();

            foreach (var download in scheduledDownloads)
            {
                // only product files are eligible for validation
                if (download.Type != ScheduledDownloadTypes.File)
                    continue;

                // and among product files only executables are eligible for validation
                if (!extensionsWhitelist.Contains(Path.GetExtension(download.Source)))
                    continue;

                if (!validationSources.ContainsKey(download.Id))
                    validationSources.Add(download.Id, new List<string>());

                validationSources[download.Id].Add(await uriRedirectController.GetUriRedirect(download.Source));
            }

            return validationSources;
        }
    }
}
