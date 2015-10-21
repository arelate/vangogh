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
            Linux,
            Extra
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

        private void MergeDictionary<Type>(
            ref IDictionary<Type, IList<Type>> primary, 
            IDictionary<Type, IList<Type>> additional)
        {
            foreach (Type key in additional.Keys)
            {
                if (!primary.ContainsKey(key))
                    primary.Add(key, new List<Type>());

                foreach (Type value in additional[key])
                {
                    if (!primary[key].Contains(value))
                    {
                        primary[key].Add(value);
                    }
                }
            }
        }

        private async Task<IDictionary<string, IList<string>>> UpdateProductFile(
            List<DownloadEntry> downloadEntries, 
            OperatingSystems os, 
            string language = "")
        {
            IDictionary<string, IList<string>> productFiles = new Dictionary<string, IList<string>>();

            foreach (var entry in downloadEntries)
            {
                consoleController.WriteLine("{0},{1}: {2} {3}, {4}", 
                    os.ToString(), 
                    language, 
                    entry.Name, 
                    entry.Version, 
                    entry.Size);

                var fromUri = Urls.HttpsRoot + entry.ManualUrl;
                var toUriParts = entry.ManualUrl.Split(
                    new string[1] { Separators.UriPart }, 
                    StringSplitOptions.RemoveEmptyEntries);
                var productFolder = toUriParts[toUriParts.Length - 2];

                if (!productFiles.ContainsKey(productFolder))
                {
                    productFiles.Add(productFolder, new List<string>());
                }

                //productFiles[productFolder].Add()

                var filename = await fileRequestController.RequestFile(
                    fromUri, 
                    productFolder, 
                    ioController, 
                    ioController, 
                    downloadProgressReporter);

                if (!string.IsNullOrEmpty(filename))
                {
                    // only add valid filenames. 
                    // string.Empty in this case indicates unsuccessful error code
                    // which likely points to no longer available file (older version)
                    productFiles[productFolder].Add(filename);
                }

                consoleController.WriteLine(string.Empty);
            }

            return productFiles;
        }

        private async Task<IDictionary<string, IList<string>>> UpdateProductOperatingSystemFiles(
            OperatingSystemsDownloads operatingSystemDownloads)
        {
            IDictionary <string, IList<string>> productFiles = new Dictionary<string, IList<string>>();

            if (downloadOperatingSystem.HasFlag(OperatingSystems.Windows) &&
                operatingSystemDownloads.Windows != null)
            {
                var windowsFiles = await UpdateProductFile(operatingSystemDownloads.Windows,
                    OperatingSystems.Windows,
                    operatingSystemDownloads.Language);

                MergeDictionary(ref productFiles, windowsFiles);
            }

            if (downloadOperatingSystem.HasFlag(OperatingSystems.Mac) &&
                operatingSystemDownloads.Mac != null)
            {
                var macFiles = await UpdateProductFile(operatingSystemDownloads.Mac, 
                    OperatingSystems.Mac, 
                    operatingSystemDownloads.Language);

                MergeDictionary(ref productFiles, macFiles);
            }

            if (downloadOperatingSystem.HasFlag(OperatingSystems.Linux) &&
                operatingSystemDownloads.Linux != null)
            {
                var linuxFiles = await UpdateProductFile(operatingSystemDownloads.Linux, 
                    OperatingSystems.Linux, 
                    operatingSystemDownloads.Language);

                MergeDictionary(ref productFiles, linuxFiles);
            }

            return productFiles;
        }

        public async Task<IDictionary<string, IList<string>>> UpdateFiles(
            GameDetails details, 
            ICollection<string> supportedLanguages)
        {
            consoleController.WriteLine("Downloading files for product {0}...", details.Title);

            IDictionary<string, IList<string>> productFiles = new Dictionary<string, IList<string>>();

            // update game files
            foreach (var download in details.LanguageDownloads)
                if (supportedLanguages.Contains(download.Language))
                    productFiles = await UpdateProductOperatingSystemFiles(download);

            // update extras
            var extrasFiles = await UpdateProductFile(details.Extras, OperatingSystems.Extra);
            MergeDictionary(ref productFiles, extrasFiles);

            // also recursively download DLC files
            foreach (var dlc in details.DLCs)
            {
                var dlcFiles = await UpdateFiles(dlc, supportedLanguages);
                MergeDictionary(ref productFiles, dlcFiles);
            }

            return productFiles;
        }
    }
}
