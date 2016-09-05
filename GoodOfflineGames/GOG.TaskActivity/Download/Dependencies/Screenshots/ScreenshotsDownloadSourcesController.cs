using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.ImageUri;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.Dependencies.Screenshots
{
    public class ScreenshotsDownloadSourcesController : IDownloadSourcesController
    {
        private IProductTypeStorageController productStorageController;
        private IImageUriController screenshotUriController;

        public ScreenshotsDownloadSourcesController(
            IProductTypeStorageController productStorageController,
            IImageUriController screenshotUriController)
        {
            this.productStorageController = productStorageController;
            this.screenshotUriController = screenshotUriController;
        }

        public async Task<IList<string>> GetDownloadSources()
        {
            var screenshots = await productStorageController.Pull<ProductScreenshots>(ProductTypes.Screenshot);

            var screenshotsSources = new List<string>();

            foreach (var screenshot in screenshots)
                foreach (var uri in screenshot.Uris)
                    screenshotsSources.Add(screenshotUriController.ExpandUri(uri));

            return screenshotsSources;
        }
    }
}
