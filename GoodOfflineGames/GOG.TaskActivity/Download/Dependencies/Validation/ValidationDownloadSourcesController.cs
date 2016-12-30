using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.UriResolution;
using Interfaces.Data;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.Dependencies.Validation
{
    public class ValidationDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<ProductRoutes> productRoutesDataController;
        private IUriResolutionController uriResolutionController;

        private readonly List<string> extensionsWhitelist = new List<string>(4) {
            ".exe", // Windows
            ".bin", // Windows
            ".dmg", // Mac
            ".sh" // Linux
        };

        public ValidationDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<ProductRoutes> productRoutesDataController,
            IUriResolutionController uriResolutionController)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.productRoutesDataController = productRoutesDataController;
            this.uriResolutionController = uriResolutionController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            var validationSources = new Dictionary<long, IList<string>>();

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                var productRoutes = await productRoutesDataController.GetByIdAsync(id);
                if (productRoutes == null) continue;

                foreach (var downloadEntry in productDownloads.Downloads)
                {
                    // only product files are eligible for validation
                    if (downloadEntry.Type != ProductDownloadTypes.ProductFile)
                        continue;

                    // trace route for the product file
                    var resolvedUri = string.Empty;

                    foreach (var route in productRoutes.Routes)
                        if (route.Source == downloadEntry.SourceUri)
                        {
                            resolvedUri = route.Destination;
                            break;
                        }

                    // only executables are eligible for validation
                    if (!extensionsWhitelist.Contains(Path.GetExtension(resolvedUri)))
                        continue;

                    if (!validationSources.ContainsKey(id))
                        validationSources.Add(id, new List<string>());

                    validationSources[id].Add(uriResolutionController.ResolveUri(resolvedUri));
                }
            }

            return validationSources;
        }
    }
}
