using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    public class FileValidationController
    {
        private const string validationFilesContainer = "_md5";
        private IIOController ioController;
        private IConsoleController consoleController;
        private IRequestFileDelegate requestFileDelegate;

        public FileValidationController(
            IIOController ioController,
            IRequestFileDelegate requestFileDelegate,
            IConsoleController consoleController)
        {
            this.ioController = ioController;
            this.consoleController = consoleController;
            this.requestFileDelegate = requestFileDelegate;
        }

        public Uri GetValidationUri(string resolvedUri)
        {
            const string replaceEntry = "?";
            const string replaceWithEntry = ".xml";

            if (string.IsNullOrEmpty(resolvedUri)) return null;

            var validationUri = (resolvedUri.Contains(replaceEntry)) ?
                new Uri(resolvedUri.Replace(replaceEntry, replaceWithEntry + replaceEntry)) :
                new Uri(resolvedUri + replaceWithEntry);

            return validationUri;
        }

        public string GetLocalValidationFilename(Uri validationUri)
        {
            return Path.Combine(validationFilesContainer,
                validationUri.Segments.Last());
        }

        public async Task<bool> DownloadValidationFile(Uri validationUri)
        {
            string validationFilename = GetLocalValidationFilename(validationUri);
            if (ioController.FileExists(validationFilename))
            {
                return true;
            }
            else
            {
                var result = await requestFileDelegate.RequestFile(
                    validationUri.ToString(),
                    validationFilesContainer,
                    ioController);

                return result.Item1;
            }
        }

        public async Task<bool> ValidateProductFile(ProductFile productFile)
        {
            if (productFile == null) return false;

            var validationUri = GetValidationUri(productFile.ResolvedUrl);

            if (await DownloadValidationFile(validationUri))
            {
                var localValidationFilename = GetLocalValidationFilename(validationUri);
            }
            else
            {
                // there is no validation file available and one couldn't be downloaded
            }

            //var validationFilename = validationUri.Segments.Last();



            return false;
        }
    }
}
