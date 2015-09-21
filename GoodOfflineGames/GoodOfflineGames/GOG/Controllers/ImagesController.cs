using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GOG.Interfaces;
using GOG.Model;
using GOG.SharedModels;

namespace GOG.Controllers
{
    public class ImagesController
    {
        private const int standardWidth = 196;
        private const int retinaWidth = standardWidth * 2;
        private int[] imageWidths = new int[2] { standardWidth, retinaWidth };
        private string imageFilenameTemplate = "{0}_{1}.jpg";
        private string imagesCacheFolder = "_images";

        private IIOController ioController;
        private IConsoleController consoleController;
        private IFileRequestController fileRequestController;

        public ImagesController(IFileRequestController fileRequestController, IIOController ioController, IConsoleController consoleController)
        {
            this.fileRequestController = fileRequestController;
            this.ioController = ioController;
            this.consoleController = consoleController;
        }

        public async Task Update(IList<Product> products)
        {
            consoleController.Write("Updating cached images...");

            await CacheImages(ExpandImagesUris(products));

            consoleController.WriteLine("DONE.");
        }

        private IEnumerable<string> ExpandImagesUris(IList<Product> products)
        {
            foreach (var product in products)
            {
                var baseUri = product.Image;
                if (!baseUri.StartsWith(Urls.HttpProtocol))
                {
                    baseUri = Urls.HttpProtocol + baseUri;
                }

                foreach (int width in imageWidths)
                {
                    yield return string.Format(imageFilenameTemplate, baseUri, width);
                }
            }
        }

        private async Task CacheImages(IEnumerable<string> uris)
        {
            if (!ioController.ExistsDirectory(imagesCacheFolder))
            {
                ioController.CreateDirectory(imagesCacheFolder);
            }

            foreach (string imageRelativeUri in uris)
            {
                var imageUri = new Uri(imageRelativeUri, UriKind.Absolute);
                var localFilename = imageUri.Segments.Last();
                var localPath = Path.Combine(imagesCacheFolder, localFilename);

                if (ioController.ExistsFile(localPath))
                {
                    continue;
                }

                consoleController.Write(".");

                await fileRequestController.RequestFile(imageUri.ToString(), imagesCacheFolder, ioController);
            }
        }

    }
}
