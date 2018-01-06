using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;

using Interfaces.Controllers.Network;

using Interfaces.Serialization;
using Interfaces.Language;
using Interfaces.Extraction;
using Interfaces.Sanitization;
using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized
{
    public class GetDeserializedGameDetailsAsyncDelegate : IGetDeserializedAsyncDelegate<GameDetails>
    {
        private IGetResourceAsyncDelegate getResourceAsyncDelegate;
        private ISerializationController<string> serializationController;
        private ILanguageController languageController;

        private IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate;
        private IStringExtractionController languagesExtractionController;
        private IStringExtractionController downloadsExtractionController;
        private ISanitizationController sanitizationController;

        private IExtractMultipleDelegate<
            IEnumerable<string>, 
            OperatingSystemsDownloads[][], 
            OperatingSystemsDownloads> 
            operatingSystemsDownloadsExtractionController;

        public GetDeserializedGameDetailsAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            ISerializationController<string> serializationController,
            ILanguageController languageController,
            IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate,
            IStringExtractionController languagesExtractionController,
            IStringExtractionController downloadsExtractionController,
            ISanitizationController sanitizationController,
            IExtractMultipleDelegate<IEnumerable<string>, OperatingSystemsDownloads[][], OperatingSystemsDownloads> operatingSystemsDownloadsExtractionController)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.serializationController = serializationController;
            this.languageController = languageController;

            this.confirmStringContainsLanguageDownloadsDelegate = confirmStringContainsLanguageDownloadsDelegate;
            this.languagesExtractionController = languagesExtractionController;
            this.downloadsExtractionController = downloadsExtractionController;
            this.sanitizationController = sanitizationController;

            this.operatingSystemsDownloadsExtractionController = operatingSystemsDownloadsExtractionController;
        }

        public async Task<GameDetails> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            // GOG.com quirk
            // GameDetails as sent by GOG.com servers have an intersting data structure for downloads:
            // it's represented as an array, where first element is a string with the language,
            // followed by actual download details, something like:
            // [ "English", { // download1 }, { // download2 } ]
            // Which of course is not a problem for JavaScript, but is a significant problem for
            // deserializing into strongly typed C# data structures. This wrapped encapsulated normal network requests,
            // data transformation and desearialization in a sinlge package. To process downloads we do the following:
            // - if response contains language downloads, signified by [[
            // - extract actual language information and remove it from the string
            // - deserialize downloads into OperatingSystemsDownloads collection
            // - assign languages, since we know we should have as many downloads array as languages

            var data = await getResourceAsyncDelegate.GetResourceAsync(status, uri, parameters);
            var gameDetails = serializationController.Deserialize<GameDetails>(data);

            if (gameDetails == null) return null;

            var downloadStrings = new List<string>();
            var gameDetailsLanguageDownloads = new List<OperatingSystemsDownloads>();

            var nullString = "null";

            while (confirmStringContainsLanguageDownloadsDelegate.Confirm(data))
            {
                var extractedDownloadStrings = downloadsExtractionController.ExtractMultiple(data);
                var downloadString = extractedDownloadStrings.Single();

                downloadStrings.Add(downloadString);

                // ...and sanitize it from original string
                data = sanitizationController.SanitizeMultiple(data, nullString, downloadString);
            }

            foreach (var downloadString in downloadStrings)
            {
                var languages = languagesExtractionController.ExtractMultiple(downloadString);
                if (languages == null)
                    throw new InvalidOperationException("Download string doesn't seem to contain any languages.");

                // ... and sanitize lanugage strings from downloads
                var sanitizedDownloadsString = sanitizationController.SanitizeMultiple(
                    downloadString, 
                    string.Empty, 
                    languages.ToArray());

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    serializationController.Deserialize<OperatingSystemsDownloads[][]>(
                    sanitizedDownloadsString);

                // and convert GOG multidimensional array of downloads to linear list using extracted languages
                var languageDownloads = operatingSystemsDownloadsExtractionController.ExtractMultiple(
                    languages,
                    downloads);

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();

            return gameDetails;
        }
    }
}
