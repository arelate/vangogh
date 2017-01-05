using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.UriResolution;
using Interfaces.Data;
using Interfaces.Routing;
using Interfaces.Eligibility;

using Models.ProductDownloads;

namespace GOG.TaskActivities.Download.Dependencies.Validation
{
    public class ValidationDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IRoutingController routingController;
        private IEligibilityDelegate<ProductDownloadEntry> downloadEntryValidationEligibilityDelegate;
        private IEligibilityDelegate<string> fileValidationEligibilityDelegate;
        private IUriResolutionController uriResolutionController;

        public ValidationDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IRoutingController routingController,
            IUriResolutionController uriResolutionController,
            IEligibilityDelegate<ProductDownloadEntry> downloadEntryValidationEligibilityDelegate,
            IEligibilityDelegate<string> fileValidationEligibilityDelegate)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.routingController = routingController;
            this.uriResolutionController = uriResolutionController;
            this.downloadEntryValidationEligibilityDelegate = downloadEntryValidationEligibilityDelegate;
            this.fileValidationEligibilityDelegate = fileValidationEligibilityDelegate;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            var validationSources = new Dictionary<long, IList<string>>();

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                foreach (var downloadEntry in productDownloads.Downloads)
                {
                    // only product files are eligible for validation
                    if (!downloadEntryValidationEligibilityDelegate.IsEligible(downloadEntry))
                        continue;

                    // trace route for the product file
                    var resolvedUri = await routingController.TraceRouteAsync(id, downloadEntry.SourceUri);

                    if (!fileValidationEligibilityDelegate.IsEligible(resolvedUri))
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
