using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Itemize;

using Attributes;

using Interfaces.Language;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeGameDetailsDownloadLanguagesDelegate : IItemizeDelegate<string, string>
    {
        ILanguageController languageController;
        IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        [Dependencies(
            "Controllers.Language.LanguageController,Controllers",
            "Delegates.Replace.ReplaceMultipleStringsDelegate,Delegates")]
        public ItemizeGameDetailsDownloadLanguagesDelegate(
            ILanguageController languageController,
            IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate)
        {
            this.languageController = languageController;
            this.replaceMultipleStringsDelegate = replaceMultipleStringsDelegate;
        }

        public IEnumerable<string> Itemize(string data)
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
    }
}
