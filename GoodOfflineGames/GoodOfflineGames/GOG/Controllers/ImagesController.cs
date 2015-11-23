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
        private const int noSpecificWidth = 0;
        private int[] imageWidths = new int[4] { standardWidth, retinaWidth, largeWidth, largeRetinaWidth };
        private string imageFilenameTemplate = "{0}_{1}.jpg";
        private string largeRetinaFilenameTemplate = "{0}.jpg";

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

        public async Task Update(IDictionary<long, List<string>> screenshots, string folder)
        {
            await CacheImages(ExpandScreenshotsUris(screenshots), folder);
        }

        public async Task Update(IEnumerable<Product> products, string folder)
        {
            await CacheImages(ExpandProductImagesUris(products), folder);
        }

        public async Task Update(IEnumerable<ProductData> productData, string folder)
        {
            await CacheImages(ExpandProductDataImagesUris(productData), folder);
        }

        private string FormatTemplate(string uri, int width)
        {
            string template = (width == largeRetinaWidth) ?
                largeRetinaFilenameTemplate :
                imageFilenameTemplate;
            return (width == largeRetinaWidth) ?
                string.Format(template, uri) :
                string.Format(template, uri, width);
        }

        private IEnumerable<string> ExpandImageUri(string image, params int[] widths)
        {
            var baseUri = image;
            if (!baseUri.StartsWith(Urls.HttpProtocol))
            {
                baseUri = Urls.HttpProtocol + baseUri;
            }

            foreach (int width in widths)
            {
                if (width == noSpecificWidth) yield return baseUri;
                else yield return FormatTemplate(baseUri, width);
            }
        }

        private IEnumerable<string> ExpandProductImagesUris(IEnumerable<Product> products)
        {
            foreach (var product in products)
                foreach (string uri in ExpandImageUri(product.Image, imageWidths))
                    yield return uri;
        }

        private IEnumerable<string> ExpandProductDataImagesUris(IEnumerable<ProductData> productData)
        {
            foreach (var data in productData)
            {
                foreach (string uri in ExpandImageUri(data.Image, imageWidths))
                    yield return uri;

                if (data.DLCs == null) continue;

                foreach (string uri in ExpandProductDataImagesUris(data.DLCs))
                    yield return uri;
            }
        }

        private IEnumerable<string> ExpandScreenshotsUris(IDictionary<long, List<string>> screenshots)
        {
            foreach (var productScreenshots in screenshots)
            {
                if (productScreenshots.Value == null) continue;
                foreach (var uri in productScreenshots.Value)
                    foreach (var eUri in ExpandImageUri(uri, noSpecificWidth))
                        yield return eUri;
            }
        }

        private async Task CacheImages(IEnumerable<string> uris, string folder)
        {
            if (!ioController.DirectoryExists(folder))
            {
                ioController.CreateDirectory(folder);
            }

            foreach (string imageRelativeUri in uris)
            {
                var imageUri = new Uri(imageRelativeUri, UriKind.Absolute);
                var localFilename = imageUri.Segments.Last();
                var localPath = Path.Combine(folder, localFilename);

                if (ioController.FileExists(localPath))
                {
                    continue;
                }

                if (postUpdateDelegate != null)
                    postUpdateDelegate.PostUpdate();

                await fileRequestController.RequestFile(imageUri.ToString(), folder, ioController);
            }
        }

    }
}
