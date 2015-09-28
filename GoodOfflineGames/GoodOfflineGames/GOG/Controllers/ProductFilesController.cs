using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;
using GOG.SharedModels;

namespace GOG.Controllers
{
    class ProductFilesController
    {
        [Flags]
        private enum OperatingSystems
        {
            Windows,
            Mac,
            Linux
        }

        private IFileRequestController fileRequestController;
        private IIOController ioController;
        private IConsoleController consoleController;

        // Windows and Mac at this point, should make configuratble in the future
        private static OperatingSystems downloadOperatingSystem = OperatingSystems.Windows | OperatingSystems.Mac;

        private IProgress<double> downloadProgressReporter;
        private string productLocation = string.Empty;

        public ProductFilesController(
            IFileRequestController fileRequestController,
            IIOController ioController,
            IConsoleController consoleController,
            IProgress<double> downloadProgressReporter)
        {
            this.downloadProgressReporter = downloadProgressReporter;

            this.fileRequestController = fileRequestController;
            this.ioController = ioController;
            this.consoleController = consoleController;
        }

        private async Task UpdateProductFile(DownloadEntry downloadEntry)
        {
            consoleController.WriteLine("{0} ({1})", downloadEntry.Name, downloadEntry.Size);

            var fromUri = Urls.HttpsRoot + downloadEntry.ManualUrl;
            var toUriParts = downloadEntry.ManualUrl.Split(new string[1] { Separators.UriPart }, StringSplitOptions.RemoveEmptyEntries);
            var productFolder = toUriParts[toUriParts.Length - 2];

            await fileRequestController.RequestFile(fromUri, productFolder, ioController, ioController, downloadProgressReporter);

            consoleController.WriteLine(string.Empty);
        }

        private async Task UpdateProductOperatingSystemFiles(OperatingSystemsDownloads operatingSystemDownloads)
        {
            List<DownloadEntry> downloadEntries = new List<DownloadEntry>();

            if (downloadOperatingSystem.HasFlag(OperatingSystems.Windows) &&
                operatingSystemDownloads.Windows != null)
                downloadEntries.AddRange(operatingSystemDownloads.Windows);

            if (downloadOperatingSystem.HasFlag(OperatingSystems.Mac) &&
                operatingSystemDownloads.Mac != null)
                downloadEntries.AddRange(operatingSystemDownloads.Mac);

            if (downloadOperatingSystem.HasFlag(OperatingSystems.Linux) &&
                operatingSystemDownloads.Linux != null)
                downloadEntries.AddRange(operatingSystemDownloads.Linux);

            foreach (DownloadEntry downloadEntry in downloadEntries)
                await UpdateProductFile(downloadEntry);
        }

        public async Task UpdateFiles(GameDetails details, ICollection<string> supportedLanguages)
        {
            consoleController.WriteLine("Downloading files for product {0}...", details.Title);
            consoleController.WriteLine(string.Empty);

            // update game files
            foreach (var download in details.LanguageDownloads)
                if (supportedLanguages.Contains(download.Language))
                    await UpdateProductOperatingSystemFiles(download);

            // update extras
            foreach (DownloadEntry extraEntry in details.Extras)
                await UpdateProductFile(extraEntry);

            // also recursively download DLC files
            foreach (var dlc in details.DLCs)
                await UpdateFiles(dlc, supportedLanguages);
        }
    }
}
