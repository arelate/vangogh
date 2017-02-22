using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Language;
using Interfaces.Containment;
using Interfaces.Extraction;
using Interfaces.Sanitization;

using GOG.Models;

namespace GOG.Controllers.Network
{
    public class GetDeserializedGameDetailsDelegate : IGetDeserializedDelegate<GameDetails>
    {
        private IGetDelegate getDelegate;
        private ISerializationController<string> serializationController;
        private ILanguageController languageController;

        private IContainmentController<string> languageDownloadsContainmentController;
        private IExtractionController languagesExtractionController;
        private IExtractionController downloadsExtractionController;
        private ISanitizationController sanitizationController;

        public GetDeserializedGameDetailsDelegate(
            IGetDelegate getDelegate,
            ISerializationController<string> serializationController,
            ILanguageController languageController,
            IContainmentController<string> languageDownloadsContainmentController,
            IExtractionController languagesExtractionController,
            IExtractionController downloadsExtractionController,
            ISanitizationController sanitizationController)
        {
            this.getDelegate = getDelegate;
            this.serializationController = serializationController;
            this.languageController = languageController;

            this.languageDownloadsContainmentController = languageDownloadsContainmentController;
            this.languagesExtractionController = languagesExtractionController;
            this.downloadsExtractionController = downloadsExtractionController;
            this.sanitizationController = sanitizationController;
        }

        public List<OperatingSystemsDownloads> ExtractLanguageDownloads(
            OperatingSystemsDownloads[][] downloads,
            IEnumerable<string> languages)
        {
            if (downloads?.Length != languages?.Count())
                throw new InvalidOperationException("Extracted different number of downloads and languages.");

            var osDownloads = new List<OperatingSystemsDownloads>();

            for (var ii = 0; ii < languages.Count(); ii++)
            {
                var download = downloads[ii]?[0];
                if (download == null)
                    throw new InvalidOperationException("Extracted downloads doesn't contain expected element");

                var language = sanitizationController.SanitizeMultiple(
                    languages.ElementAt(ii),
                    string.Empty,
                    new string[2] { "\"", "," });

                language = Regex.Unescape(language);
                var languageCode = languageController.GetLanguageCode(language);

                download.Language = languageCode;

                osDownloads.Add(download);
            }

            return osDownloads;
        }

        public async Task<GameDetails> GetDeserialized(string uri, IDictionary<string, string> parameters = null)
        {
            var data = await getDelegate.Get(uri, parameters);
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
                var languageDownloads = ExtractLanguageDownloads(
                                    downloads,
                                    languages);

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();

            return gameDetails;
        }
    }
}
