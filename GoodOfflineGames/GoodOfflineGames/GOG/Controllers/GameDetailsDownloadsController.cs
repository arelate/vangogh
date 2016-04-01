using System;
using System.Linq;
using System.Collections.Generic;
using GOG.Interfaces;
using System.Text.RegularExpressions;

using GOG.Model;

namespace GOG.Controllers
{
    public class GameDetailsDownloadsController : 
        IGameDetailsDownloadsController
    {
        private const string downloadsStart = "[[";
        private const string downloadsEnd = "]]";
        private const string nullString = "null";
        private const string emptyString = "";

        public string ExtractSingle(string input)
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

        public string SanitizeSingle(string inputString, string sanitizedValue)
        {
            return inputString.Replace(sanitizedValue, nullString);
        }

        public string SanitizeMultiple(string inputString, IEnumerable<string> sanitizedValues)
        {
            foreach (var sanitizedValue in sanitizedValues)
                inputString = inputString.Replace(sanitizedValue, emptyString);

            return inputString;
        }

        public IEnumerable<string> ExtractMultiple(string data)
        {
            const string languagePattern = @"\[""[\w\\]*"",";
            var regex = new Regex(languagePattern);

            var match = regex.Match(data);
            while (match.Success)
            {
                yield return match.Value.Substring(1);
                match = match.NextMatch();
            }
        }

        public GameDetails ExtractLanguageDownloads(
            GameDetails details, 
            OperatingSystemsDownloads[][] downloads, 
            IEnumerable<string> languages)
        {
            if (downloads?.Length != languages?.Count())
                throw new InvalidOperationException("Extracted different number of downloads and languages.");

            details.LanguageDownloads = new List<OperatingSystemsDownloads>();

            for (var ii = 0; ii < languages.Count(); ii++)
            {
                var download = downloads[ii]?[0];
                if (download == null)
                    throw new InvalidOperationException("Extracted downloads doesn't contain expected element");

                var language = SanitizeMultiple(
                    languages.ElementAt(ii),
                    new string[2] { "\"", "," });

                download.Language = language;

                details?.LanguageDownloads.Add(download);
            }

            return details;
        }
    }
}
