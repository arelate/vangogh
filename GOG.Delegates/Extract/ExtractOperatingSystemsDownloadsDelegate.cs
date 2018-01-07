using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Interfaces.Delegates.Replace;

using Interfaces.Language;

using Interfaces.Extraction;

using GOG.Models;

namespace GOG.Delegates.Extract
{
    // TODO: untangle this single instance of triple type ExtractMultiple
    public class ExtractOperatingSystemsDownloadsDelegate: 
        IExtractMultipleDelegate<IEnumerable<string>, OperatingSystemsDownloads[][], OperatingSystemsDownloads>
    {
        private IReplaceMultipleDelegate replaceMultipleDelegate;
        private ILanguageController languageController;

        public ExtractOperatingSystemsDownloadsDelegate(
            IReplaceMultipleDelegate replaceMultipleDelegate,
            ILanguageController languageController)
        {
            this.replaceMultipleDelegate = replaceMultipleDelegate;
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

                var language = replaceMultipleDelegate.ReplaceMultiple(
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
