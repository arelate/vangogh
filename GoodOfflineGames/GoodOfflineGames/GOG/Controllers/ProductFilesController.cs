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
        private IFileRequestController fileRequestController;
        private IIOController ioController;
        private IConsoleController consoleController;

        private const string Windows = "Windows";
        private const string Mac = "Mac";
        private const string Linux = "Linux";

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
                    if (!primary[key].Contains(value))
                        primary[key].Add(value);
            }
        }

        private async Task<Tuple<bool, IDictionary<string, IList<string>>>> UpdateProductFile(
            List<DownloadEntry> downloadEntries,
            string operatingSystem = "",
            string language = "")
        {
            IDictionary<string, IList<string>> productFiles = new Dictionary<string, IList<string>>();
            var success = true;

            foreach (var entry in downloadEntries)
            {
                string entryMessage = (string.IsNullOrEmpty(operatingSystem) && string.IsNullOrEmpty(language)) ?
                    string.Format("{0} ({1})", entry.Name, entry.Size) :
                    string.Format("{0} {1} ({2}, {3}, {4})",
                        entry.Name,
                        entry.Version,
                        operatingSystem,
                        language,
                        entry.Size);

                consoleController.WriteLine(entryMessage);

                var fromUri = Urls.HttpsRoot + entry.ManualUrl;
                var toUriParts = entry.ManualUrl.Split(
                    new string[1] { Separators.UriPart },
                    StringSplitOptions.RemoveEmptyEntries);
                var productFolder = toUriParts[toUriParts.Length - 2];

                if (!productFiles.ContainsKey(productFolder))
                {
                    productFiles.Add(productFolder, new List<string>());
                }

                var result = await fileRequestController.RequestFile(
                    fromUri,
                    productFolder,
                    ioController,
                    ioController,
                    downloadProgressReporter,
                    consoleController);

                success &= result.Item1;
                var filename = result.Item2;

                consoleController.WriteLine(string.Empty);

                if (!string.IsNullOrEmpty(filename))
                {
                    // only add valid filenames. 
                    // string.Empty in this case indicates unsuccessful error code
                    // which likely points to no longer available file (older version)
                    productFiles[productFolder].Add(filename);
                }
            }

            return new Tuple<bool, IDictionary<string, IList<string>>>(success, productFiles);
        }

        private async Task<Tuple<bool, IDictionary<string, IList<string>>>> UpdateProductOperatingSystemFiles(
            OperatingSystemsDownloads operatingSystemDownloads,
            ICollection<string> downloadOperatingSystems)
        {
            IDictionary<string, IList<string>> productFiles = new Dictionary<string, IList<string>>();
            var success = true;

            if (downloadOperatingSystems.Contains(Windows) &&
                operatingSystemDownloads.Windows != null)
            {
                var windowsFilesResult = await UpdateProductFile(operatingSystemDownloads.Windows,
                    Windows,
                    operatingSystemDownloads.Language);

                MergeDictionary(ref productFiles, windowsFilesResult.Item2);
                success &= windowsFilesResult.Item1;
            }

            if (downloadOperatingSystems.Contains(Mac) &&
                operatingSystemDownloads.Mac != null)
            {
                var macFilesResult = await UpdateProductFile(operatingSystemDownloads.Mac,
                    Mac,
                    operatingSystemDownloads.Language);

                MergeDictionary(ref productFiles, macFilesResult.Item2);
                success &= macFilesResult.Item1;
            }

            if (downloadOperatingSystems.Contains(Linux) &&
                operatingSystemDownloads.Linux != null)
            {
                var linuxFilesResult = await UpdateProductFile(operatingSystemDownloads.Linux,
                    Linux,
                    operatingSystemDownloads.Language);

                MergeDictionary(ref productFiles, linuxFilesResult.Item2);
                success &= linuxFilesResult.Item1;
            }

            return new Tuple<bool, IDictionary<string, IList<string>>>(success, productFiles);
        }

        public async Task<Tuple<bool, IDictionary<string, IList<string>>>> UpdateFiles(
            GameDetails details,
            ICollection<string> supportedLanguages,
            ICollection<string> supportedOperatingSystems)
        {
            consoleController.WriteLine("Downloading files for product {0}...", details.Title);

            IDictionary<string, IList<string>> productFiles = new Dictionary<string, IList<string>>();
            var success = true;

            // update game files
            foreach (var download in details.LanguageDownloads)
                if (supportedLanguages.Contains(download.Language))
                {
                    var productFilesResult = await UpdateProductOperatingSystemFiles(download, supportedOperatingSystems);
                    productFiles = productFilesResult.Item2;
                    success &= productFilesResult.Item1;
                }

            // update extras
            var extrasFilesResult = await UpdateProductFile(details.Extras, string.Empty);
            MergeDictionary(ref productFiles, extrasFilesResult.Item2);
            success &= extrasFilesResult.Item1;

            // also recursively download DLC files
            foreach (var dlc in details.DLCs)
            {
                var dlcFilesResult = await UpdateFiles(dlc, supportedLanguages, supportedOperatingSystems);
                MergeDictionary(ref productFiles, dlcFilesResult.Item2);
                success &= dlcFilesResult.Item1;
            }

            return new Tuple<bool, IDictionary<string, IList<string>>>(success, productFiles);
        }
    }
}
