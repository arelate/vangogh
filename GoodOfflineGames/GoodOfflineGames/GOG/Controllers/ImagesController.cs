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
        private const int largeWidth = 800;
        private const int largeRetinaWidth = 1600;
        private int[] imageWidths = new int[4] { standardWidth, retinaWidth, largeWidth, largeRetinaWidth };
        private string imageFilenameTemplate = "{0}_{1}.jpg";
        private string largeRetinaImageFilenameTemplate = "{0}.jpg";
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
            await CacheImages(ExpandProductImagesUris(products));
        }

        public async Task Update(IEnumerable<ProductData> productData)
        {
            await CacheImages(ExpandProductDataImagesUris(productData));
        }

        private string FormatTemplate(string uri, int width)
        {
            string template = (width == largeRetinaWidth) ? largeRetinaImageFilenameTemplate : imageFilenameTemplate;
            return (width == largeRetinaWidth) ? string.Format(template, uri) : string.Format(template, uri, width);
        }

        private IEnumerable<string> ExpandImageUri(string image)
        {
            var baseUri = image;
            if (!baseUri.StartsWith(Urls.HttpProtocol))
            {
                baseUri = Urls.HttpProtocol + baseUri;
            }

            foreach (int width in imageWidths)
            {
                yield return FormatTemplate(baseUri, width);
            }
        }

        private IEnumerable<string> ExpandProductImagesUris(IEnumerable<Product> products)
        {
            foreach (var product in products)
                foreach (string uri in ExpandImageUri(product.Image))
                    yield return uri;
        }

        private IEnumerable<string> ExpandProductDataImagesUris(IEnumerable<ProductData> productData)
        {
            foreach (var data in productData)
            {
                foreach (string uri in ExpandImageUri(data.Image))
                    yield return uri;

                if (data.DLCs == null) continue;

                foreach (string uri in ExpandProductDataImagesUris(data.DLCs))
                    yield return uri;
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
