//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//using GOG.Interfaces;
//using GOG.Model;

//using Models.Uris;
//using Models.Separators;

//namespace GOG.Controllers
//{
//    public class ProductFilesDownloadController: IProductFilesDownloadController
//    {
//        private IRequestFileDelegate requestFileDelegate;
//        private IIOController ioController;
//        private IConsoleController consoleController;

//        private const string Windows = "Windows";
//        private const string Mac = "Mac";
//        private const string Linux = "Linux";

//        private IDownloadProgressReportingController downloadProgressReportingController;
//        private string productLocation = string.Empty;

//        public ProductFilesDownloadController(
//            IRequestFileDelegate requestFileDelegate,
//            IIOController ioController,
//            IConsoleController consoleController,
//            IDownloadProgressReportingController downloadProgressReportingController)
//        {
//            this.downloadProgressReportingController = downloadProgressReportingController;

//            this.requestFileDelegate = requestFileDelegate;
//            this.ioController = ioController;
//            this.consoleController = consoleController;
//        }

//        public async Task<ProductFile> UpdateDownloadEntry(
//            DownloadEntry downloadEntry,
//            long id,
//            string operatingSystem = "",
//            string language = "")
//        {
//            var productFile = new ProductFile();
//            productFile.Id = id;
//            productFile.OperatingSystem = operatingSystem;
//            productFile.Language = language;
//            productFile.Name = downloadEntry.Name;
//            productFile.Version = downloadEntry.Version;
//            productFile.Size = downloadEntry.Size;

//            // Extras are generally OS and language agnostic - so instead of specific flag we use empty OS, language
//            productFile.Extra =
//                string.IsNullOrEmpty(operatingSystem) &&
//                string.IsNullOrEmpty(language);

//            string entryMessage = (string.IsNullOrEmpty(operatingSystem) && string.IsNullOrEmpty(language)) ?
//                string.Format("{0} ({1})", downloadEntry.Name, downloadEntry.Size) :
//                string.Format("{0} {1} ({2}, {3}, {4})",
//                    downloadEntry.Name,
//                    downloadEntry.Version,
//                    operatingSystem,
//                    language,
//                    downloadEntry.Size);

//            consoleController.WriteLine(entryMessage, ConsoleColor.Gray);

//            var fromUri = Uris.Protocols.Https + Uris.Roots.Website + downloadEntry.ManualUrl;
//            var toUriParts = downloadEntry.ManualUrl.Split(
//                new string[1] { Separators.UriPart },
//                StringSplitOptions.RemoveEmptyEntries);
//            productFile.Url = downloadEntry.ManualUrl;
//            productFile.Folder = toUriParts[toUriParts.Length - 2];

//            var result = await requestFileDelegate.RequestFile(
//                fromUri,
//                productFile.Folder,
//                ioController,
//                ioController,
//                downloadProgressReportingController,
//                consoleController);

//            productFile.DownloadSuccessful = result.Item1;
//            var fileUri = result.Item2;
//            productFile.File = fileUri.Segments.Last();
//            productFile.ResolvedUrl = fileUri.ToString();

//            consoleController.WriteLine(string.Empty, ConsoleColor.Gray);

//            return productFile;
//        }

//        public async Task<IList<ProductFile>> UpdateProductFiles(
//            List<DownloadEntry> downloadEntries,
//            long id,
//            string operatingSystem = "",
//            string language = "")
//        {
//            var productFiles = new List<ProductFile>();

//            foreach (var entry in downloadEntries)
//            {
//                var productFile = await UpdateDownloadEntry(entry, id, operatingSystem, language);
//                productFiles.Add(productFile);
//            }

//            return productFiles;
//        }

//        public async Task<IList<ProductFile>> UpdateProductOperatingSystemFiles(
//            OperatingSystemsDownloads operatingSystemDownloads,
//            ICollection<string> downloadOperatingSystems,
//            long id)
//        {
//            List<ProductFile> productFiles = new List<ProductFile>();

//            if (downloadOperatingSystems.Contains(Windows) &&
//                operatingSystemDownloads.Windows != null)
//            {
//                var windowsFiles = await UpdateProductFiles(operatingSystemDownloads.Windows,
//                    id,
//                    Windows,
//                    operatingSystemDownloads.Language);

//                productFiles.AddRange(windowsFiles);
//            }

//            if (downloadOperatingSystems.Contains(Mac) &&
//                operatingSystemDownloads.Mac != null)
//            {
//                var macFiles = await UpdateProductFiles(operatingSystemDownloads.Mac,
//                    id,
//                    Mac,
//                    operatingSystemDownloads.Language);

//                productFiles.AddRange(macFiles);
//            }

//            if (downloadOperatingSystems.Contains(Linux) &&
//                operatingSystemDownloads.Linux != null)
//            {
//                var linuxFiles = await UpdateProductFiles(operatingSystemDownloads.Linux,
//                    id,
//                    Linux,
//                    operatingSystemDownloads.Language);

//                productFiles.AddRange(linuxFiles);
//            }

//            return productFiles;
//        }


//        public async Task<IList<ProductFile>> UpdateFiles(
//            GameDetails details,
//            ICollection<string> requiredLanguageCodes,
//            ICollection<string> supportedOperatingSystems,
//            long context = 0)
//        {
//            consoleController.WriteLine("Downloading files for product {0}...", ConsoleColor.White, details.Title);

//            List<ProductFile> productFiles = new List<ProductFile>();

//            var currentContext = details.Id > 0 ? details.Id : context;

//            // update game files, check if LanguagesDownloads are null to account for the DLC files 
//            // that were extracted earlier and should be part of the core product file details
//            // don't bail out earlier as there might be extras specific to DLC we would want to handle
//            if (details.LanguageDownloads != null)
//            {
//                foreach (var download in details.LanguageDownloads)
//                    if (requiredLanguageCodes.Contains(download.Language))
//                    {
//                        var productInstallers = await UpdateProductOperatingSystemFiles(
//                            download,
//                            supportedOperatingSystems,
//                            currentContext);

//                        productFiles.AddRange(productInstallers);
//                    }
//            }

//            // update extras
//            var extraFiles = await UpdateProductFiles(details.Extras, currentContext);
//            productFiles.AddRange(extraFiles);

//            // also recursively download DLC files
//            foreach (var dlc in details.DLCs)
//            {
//                // propagate parent product for DLCs of DLCs

//                var dlcFiles = await UpdateFiles(
//                    dlc,
//                    requiredLanguageCodes, 
//                    supportedOperatingSystems,
//                    currentContext);

//                productFiles.AddRange(dlcFiles);
//            }

//            return productFiles;
//        }
//    }
//}
