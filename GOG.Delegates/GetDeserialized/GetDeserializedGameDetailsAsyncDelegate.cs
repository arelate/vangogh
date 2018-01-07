using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Network;

using Interfaces.Serialization;
using Interfaces.Language;
using Interfaces.Status;
using Interfaces.Collection;

using GOG.Interfaces.Delegates.GetDeserialized;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized
{
    public class GetDeserializedGameDetailsAsyncDelegate : IGetDeserializedAsyncDelegate<GameDetails>
    {
        private IGetResourceAsyncDelegate getResourceAsyncDelegate;
        private ISerializationController<string> serializationController;
        private ILanguageController languageController;
        private IFormatDelegate<string, string> formatDownloadLanguageDelegate;
        private IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate;
        private IItemizeDelegate<string, string> itemizeDownloadLanguagesDelegate;
        private IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate;
        private IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;
        private IConvertDelegate<
            OperatingSystemsDownloads[][], 
            OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate;
        private ICollectionController collectionController;

        public GetDeserializedGameDetailsAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            ISerializationController<string> serializationController,
            ILanguageController languageController,
            IFormatDelegate<string, string> formatDownloadLanguageDelegate,
            IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate,
            IItemizeDelegate<string, string> itemizeDownloadLanguagesDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate,
            IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate,
            IConvertDelegate<
                OperatingSystemsDownloads[][], 
                OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate,
            ICollectionController collectionController)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.serializationController = serializationController;
            this.languageController = languageController;
            this.formatDownloadLanguageDelegate = formatDownloadLanguageDelegate;

            this.confirmStringContainsLanguageDownloadsDelegate = confirmStringContainsLanguageDownloadsDelegate;
            this.itemizeDownloadLanguagesDelegate = itemizeDownloadLanguagesDelegate;
            this.itemizeGameDetailsDownloadsDelegate = itemizeGameDetailsDownloadsDelegate;
            this.replaceMultipleStringsDelegate = replaceMultipleStringsDelegate;
            this.convert2DArrayToArrayDelegate = convert2DArrayToArrayDelegate;
            this.collectionController = collectionController;
        }

        public async Task<GameDetails> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            // GOG.com quirk
            // GameDetails as sent by GOG.com servers have an intersting data structure for downloads:
            // it's represented as an array, where first element is a string with the language,
            // followed by actual download details, something like:
            // [ "English", { // download1 }, { // download2 } ]
            // Which of course is not a problem for JavaScript, but is a problem for
            // deserializing into strongly typed C# data structures. 
            // To work around this we wrapped encapsulated usual network requests,
            // data transformation and desearialization in a sinlge package. 
            // To process downloads we do the following:
            // - if response contains language downloads, signified by [[
            // - extract actual language information and remove it from the string
            // - deserialize downloads into OperatingSystemsDownloads collection
            // - assign languages, since we know we should have as many downloads array as languages

            var data = await getResourceAsyncDelegate.GetResourceAsync(status, uri, parameters);
            var gameDetails = serializationController.Deserialize<GameDetails>(data);

            if (gameDetails == null) return null;

            var gameDetailsLanguageDownloads = new List<OperatingSystemsDownloads>();

            if (!confirmStringContainsLanguageDownloadsDelegate.Confirm(data)) return gameDetails;
            var downloadStrings = itemizeGameDetailsDownloadsDelegate.Itemize(data);

            foreach (var downloadString in downloadStrings)
            {
                var downloadLanguages = itemizeDownloadLanguagesDelegate.Itemize(downloadString);
                if (downloadLanguages == null)
                    throw new InvalidOperationException("Cannot find any download languages or download language format changed.");

                // ... and remove download lanugage strings from downloads
                var downloadsStringSansLanguages = replaceMultipleStringsDelegate.ReplaceMultiple(
                    downloadString,
                    string.Empty,
                    downloadLanguages.ToArray());

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    serializationController.Deserialize<OperatingSystemsDownloads[][]>(
                    downloadsStringSansLanguages);

                // and convert GOG two-dimensional array of downloads to single-dimensional array
                var languageDownloads = convert2DArrayToArrayDelegate.Convert(downloads);

                if (languageDownloads.Count() != downloadLanguages.Count())
                    throw new InvalidOperationException("Number of extracted language downloads doesn't match number of languages.");

                // map language downloads with the language code we extracted earlier
                var languageDownloadIndex = 0;

                collectionController.Map(downloadLanguages, language =>
                {
                    var formattedLanguage = formatDownloadLanguageDelegate.Format(language);
                    var languageCode = languageController.GetLanguageCode(formattedLanguage);

                    languageDownloads[languageDownloadIndex++].Language = languageCode;
                });

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();

            return gameDetails;
        }
    }
}
