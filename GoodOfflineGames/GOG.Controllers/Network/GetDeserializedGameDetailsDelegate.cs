using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Language;
using Interfaces.Containment;
using Interfaces.Extraction;
using Interfaces.Sanitization;
using Interfaces.TaskStatus;

using GOG.Interfaces.Extraction;

using GOG.Models;

namespace GOG.Controllers.Network
{
    public class GetDeserializedGameDetailsDelegate : IGetDeserializedDelegate<GameDetails>
    {
        private IGetDelegate getDelegate;
        private ISerializationController<string> serializationController;
        private ILanguageController languageController;

        private IContainmentController<string> languageDownloadsContainmentController;
        private IStringExtractionController languagesExtractionController;
        private IStringExtractionController downloadsExtractionController;
        private ISanitizationController sanitizationController;

        private IOperatingSystemsDownloadsExtractionController operatingSystemsDownloadsExtractionController;

        public GetDeserializedGameDetailsDelegate(
            IGetDelegate getDelegate,
            ISerializationController<string> serializationController,
            ILanguageController languageController,
            IContainmentController<string> languageDownloadsContainmentController,
            IStringExtractionController languagesExtractionController,
            IStringExtractionController downloadsExtractionController,
            ISanitizationController sanitizationController,
            IOperatingSystemsDownloadsExtractionController operatingSystemsDownloadsExtractionController)
        {
            this.getDelegate = getDelegate;
            this.serializationController = serializationController;
            this.languageController = languageController;

            this.languageDownloadsContainmentController = languageDownloadsContainmentController;
            this.languagesExtractionController = languagesExtractionController;
            this.downloadsExtractionController = downloadsExtractionController;
            this.sanitizationController = sanitizationController;

            this.operatingSystemsDownloadsExtractionController = operatingSystemsDownloadsExtractionController;
        }

        public async Task<GameDetails> GetDeserialized(ITaskStatus taskStatus, string uri, IDictionary<string, string> parameters = null)
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

            var data = await getDelegate.Get(taskStatus, uri, parameters);
            var gameDetails = serializationController.Deserialize<GameDetails>(data);

            if (gameDetails == null) return null;

            var downloadStrings = new List<string>();
            var gameDetailsLanguageDownloads = new List<OperatingSystemsDownloads>();

            var nullString = "null";

            while (languageDownloadsContainmentController.Contained(data))
            {
                var extractedDownloadStrings = downloadsExtractionController.ExtractMultiple(data);
                var downloadString = extractedDownloadStrings.First();

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

                // and convert GOG multidimensional array of downloads to linear list
                operatingSystemsDownloadsExtractionController.Languages = languages;

                var languageDownloads = operatingSystemsDownloadsExtractionController.ExtractMultiple(
                                    downloads);

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();

            return gameDetails;
        }
    }
}
