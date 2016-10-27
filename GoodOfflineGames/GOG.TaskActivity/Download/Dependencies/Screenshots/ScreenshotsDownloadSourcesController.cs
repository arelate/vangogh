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
        //private IProductTypeStorageController productStorageController;
        private IImageUriController screenshotUriController;

        public ScreenshotsDownloadSourcesController(
            //IProductTypeStorageController productStorageController,
            IImageUriController screenshotUriController)
        {
            //this.productStorageController = productStorageController;
            this.screenshotUriController = screenshotUriController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var screenshots = new List<ProductScreenshots>(); // await productStorageController.Pull<ProductScreenshots>(ProductTypes.Screenshot);

            var screenshotsSources = new Dictionary<long, IList<string>>();

            foreach (var screenshot in screenshots)
            {
                screenshotsSources.Add(screenshot.Id, new List<string>());

                foreach (var uri in screenshot.Uris)
                    screenshotsSources[screenshot.Id].Add(screenshotUriController.ExpandUri(uri));
            }

            return screenshotsSources;
        }
    }
}
