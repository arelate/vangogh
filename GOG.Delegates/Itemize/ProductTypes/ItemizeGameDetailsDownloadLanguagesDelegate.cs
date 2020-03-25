using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Itemize;


using Attributes;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeGameDetailsDownloadLanguagesDelegate : IItemizeDelegate<string, string>
    {
        IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        [Dependencies(
            "Delegates.Replace.ReplaceMultipleStringsDelegate,Delegates")]
        public ItemizeGameDetailsDownloadLanguagesDelegate(
            IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate)
        {
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
