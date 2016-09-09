using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using Interfaces.AdditionalDetails;
using Interfaces.Serialization;
using Interfaces.Language;

namespace GOG.TaskActivities.Update.Dependencies.GameDetails
{
    public class GameDetailsDownloadDetailsController : IAdditionalDetailsController
    {
        private const string downloadsStart = "[[";
        private const string downloadsEnd = "]]";
        private const string nullString = "null";
        private const string emptyString = "";

        private ISerializationController<string> serializationController;
        private ILanguageController languageController;

        public GameDetailsDownloadDetailsController(
            ISerializationController<string> serializationController,
            ILanguageController languageController)
        {
            this.serializationController = serializationController;
            this.languageController = languageController;
        }

        private bool ContainsLanguageDownloads(string input)
        {
            return input.Contains(downloadsStart) &&
                input.Contains(downloadsEnd);
        }

        private string ExtractSingle(string input)
        {
            // downloads are double array and so far nothing else in the game details data is
            // so we'll leverage this fact to extract actual content

            string result = string.Empty;

            int fromIndex = input.IndexOf(downloadsStart),
                toIndex = input.IndexOf(downloadsEnd);

            if (fromIndex < toIndex)
                result = input.Substring(
                    fromIndex,
                    toIndex - fromIndex + downloadsEnd.Length);

            return result;
        }

        private string SanitizeSingle(string inputString, string sanitizedValue)
        {
            return inputString.Replace(sanitizedValue, nullString);
        }

        private string SanitizeMultiple(string inputString, IEnumerable<string> sanitizedValues)
        {
            foreach (var sanitizedValue in sanitizedValues)
                inputString = inputString.Replace(sanitizedValue, emptyString);

            return inputString;
        }

        public IEnumerable<string> ExtractMultiple(string data)
        {
            const string languagePattern = @"\[""[\w\\ ]*"",";
            var regex = new Regex(languagePattern);

            var match = regex.Match(data);
            while (match.Success)
            {
                yield return match.Value.Substring(1);
                match = match.NextMatch();
            }
        }

        public List<Models.OperatingSystemsDownloads> ExtractLanguageDownloads(
            Models.OperatingSystemsDownloads[][] downloads,
            IEnumerable<string> languages)
        {
            if (downloads?.Length != languages?.Count())
                throw new InvalidOperationException("Extracted different number of downloads and languages.");

            var osDownloads = new List<Models.OperatingSystemsDownloads>();

            for (var ii = 0; ii < languages.Count(); ii++)
            {
                var download = downloads[ii]?[0];
                if (download == null)
                    throw new InvalidOperationException("Extracted downloads doesn't contain expected element");

                var language = SanitizeMultiple(
                    languages.ElementAt(ii),
                    new string[2] { "\"", "," });

                language = Regex.Unescape(language);
                var languageCode = languageController.GetLanguageCode(language);

                download.Language = languageCode;

                osDownloads.Add(download);
            }

            return osDownloads;
        }

        public void AddDetails<Type>(Type element, string data)
        {
            var gameDetails = element as Models.GameDetails;
            if (gameDetails == null) return;

            var downloadStrings = new List<string>();
            var gameDetailsLanguageDownloads = new List<Models.OperatingSystemsDownloads>();

            while (ContainsLanguageDownloads(data))
            {
                var downloadString = ExtractSingle(data);
                downloadStrings.Add(downloadString);

                // ...and sanitize it from original string
                data = SanitizeSingle(data, downloadString);
            }

            foreach (var downloadString in downloadStrings)
            {

                var languages = ExtractMultiple(downloadString);
                if (languages == null)
                    throw new InvalidOperationException("Download string doesn't seem to contain any languages.");

                // ... and sanitize lanugage strings from downloads
                var sanitizedDownloadsString = SanitizeMultiple(downloadString, languages);

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    serializationController.Deserialize<Models.OperatingSystemsDownloads[][]>(
                    sanitizedDownloadsString);

                // and convert GOG multidimensional array of downloads to linear list
                var languageDownloads = ExtractLanguageDownloads(
                                    downloads,
                                    languages);

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();
        }
    }
}
