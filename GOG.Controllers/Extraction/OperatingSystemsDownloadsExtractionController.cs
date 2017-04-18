using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Interfaces.Sanitization;
using Interfaces.Language;

using Interfaces.Extraction;
using GOG.Interfaces.Models;
using GOG.Models;

namespace GOG.Controllers.Extraction
{
    public class OperatingSystemsDownloadsExtractionController : 
        IExtractMultipleDelegate<IEnumerable<string>, OperatingSystemsDownloads[][], OperatingSystemsDownloads>
    {
        private ISanitizationController sanitizationController;
        private ILanguageController languageController;

        public OperatingSystemsDownloadsExtractionController(
            ISanitizationController sanitizationController,
            ILanguageController languageController)
        {
            this.sanitizationController = sanitizationController;
            this.languageController = languageController;
        }

        public IEnumerable<OperatingSystemsDownloads> ExtractMultiple(IEnumerable<string> languages, OperatingSystemsDownloads[][] downloads)
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
    }
}
