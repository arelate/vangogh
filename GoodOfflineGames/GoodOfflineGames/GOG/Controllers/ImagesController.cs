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
        private IPostUpdateDelegate postUpdateDelegate;
        private IFileRequestController fileRequestController;

        public ImagesController(IFileRequestController fileRequestController,
            IIOController ioController,
            IPostUpdateDelegate postUpdateDelegate = null)
        {
            this.fileRequestController = fileRequestController;
            this.ioController = ioController;
            this.postUpdateDelegate = postUpdateDelegate;
        }

        public async Task Update(IEnumerable<Product> products)
        {
            await CacheImages(ExpandImagesUris(products));
        }

        private IEnumerable<string> ExpandImagesUris(IEnumerable<Product> products)
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
            if (!ioController.DirectoryExists(imagesCacheFolder))
            {
                ioController.CreateDirectory(imagesCacheFolder);
            }

            foreach (string imageRelativeUri in uris)
            {
                var imageUri = new Uri(imageRelativeUri, UriKind.Absolute);
                var localFilename = imageUri.Segments.Last();
                var localPath = Path.Combine(imagesCacheFolder, localFilename);

                if (ioController.FileExists(localPath))
                {
                    continue;
                }

                if (postUpdateDelegate != null)
                    postUpdateDelegate.PostUpdate();

                await fileRequestController.RequestFile(imageUri.ToString(), imagesCacheFolder, ioController);
            }
        }

    }
}
