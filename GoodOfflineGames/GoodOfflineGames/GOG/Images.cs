using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GOG
{
    public class Images
    {
        private const int standardWidth = 196;
        private const int retinaWidth = standardWidth * 2;
        private int[] imageWidths = new int[2] { standardWidth, retinaWidth };
        private string imageFilenameTemplate = "{0}_{1}.jpg";
        private string imagesCacheFolder = "_images";
        private IIOController ioController;
        private IConsoleController consoleController;

        public Images(IIOController ioController, IConsoleController consoleController)
        {
            this.ioController = ioController;
            this.consoleController = consoleController;
        }

        public async Task Update(ProductsResult result)
        {
            consoleController.Write("Updating cached images...");

            await CacheImages(ExpandImagesUris(result.Products));

            consoleController.WriteLine("DONE.");
        }

        private IEnumerable<string> ExpandImagesUris(List<Product> products)
        {
            foreach (Product product in products)
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

            foreach (string imageUri in uris)
            {
                string localFilename = GetImageLocalFilename(imageUri);

                if (ioController.ExistsFile(localFilename))
                {
                    continue;
                }

                consoleController.Write(".");

                await Network.DownloadFile(imageUri, localFilename, ioController);
            }
        }

        private string GetImageLocalFilename(string imageUri)
        {
            Uri uri = new Uri(imageUri, UriKind.Absolute);
            return Path.Combine(imagesCacheFolder, uri.Segments.Last());
        }

    }
}
