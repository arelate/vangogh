using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Itemize;

using Interfaces.Language;

namespace GOG.Delegates.Itemize
{
    public class ItemizeDownloadLanguagesDelegate : IItemizeDelegate<string, string>
    {
        private ILanguageController languageController;
        private IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        public ItemizeDownloadLanguagesDelegate(
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
