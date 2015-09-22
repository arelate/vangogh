using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;
using GOG.SharedModels;

namespace GOG.Controllers
{
    [Flags]
    enum OperatingSystems
    {
        Windows,
        Mac,
        Linux
    }

    [Flags]
    enum Languages
    {
        English
    }

    class DownloadProgressReporter : IProgress<double>
    {
        private IConsoleController consoleController;

        public DownloadProgressReporter(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public void Report(double value)
        {
            consoleController.Write("\r{0:P1}...", value);
        }
    }

    class ProductFilesController
    {
        private IFileRequestController fileRequestController;
        private IIOController ioController;
        private IConsoleController consoleController;

        // Windows and Mac at this point, should make configuratble in the future
        private static OperatingSystems downloadOperatingSystem = OperatingSystems.Windows | OperatingSystems.Mac;

        // English at this point, should make configurable in the future
        private static Languages downloadLanguage = Languages.English;

        private const bool downloadWindows = true;
        private const bool downloadMac = true;
        private const bool downloadLinux = false;

        private DownloadProgressReporter downloadProgressReporter;
        private string productLocation = string.Empty;

        public ProductFilesController(
            IFileRequestController fileRequestController,
            IIOController ioController,
            IConsoleController consoleController)
        {
            downloadProgressReporter = new DownloadProgressReporter(consoleController);

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

        private async Task UpdateProductLanguageFiles(LanguageDownloads languageDownloads)
        {
            if (downloadLanguage.HasFlag(Languages.English))
                await UpdateProductOperatingSystemFiles(languageDownloads.English);
        }

        private async Task UpdateProductFiles(GameDetails gameDetails)
        {
            //// update game files
            //await UpdateProductLanguageFiles(gameDetails.Downloads);

            //// update extras
            //foreach (DownloadEntry extraEntry in gameDetails.Extras)
            //    await UpdateProductFile(extraEntry);

            //// also recursively download DLC files
            //foreach (var dlc in gameDetails.DLCs)
            //    await UpdateProductFiles(dlc);
        }

        public async Task UpdateFiles(IEnumerable<GameDetails> details)
        {
            consoleController.WriteLine("Downloading files for {0} products...", details.Count());
            consoleController.WriteLine(string.Empty);

            foreach (GameDetails d in details)
            {
                if (d == null) continue;

                consoleController.WriteLine("Downloading {0}...", d.Title);

                // download product files
                await UpdateProductFiles(d);

                consoleController.WriteLine(string.Empty);
            }

            consoleController.WriteLine("All product files updated.");
        }
    }
}
