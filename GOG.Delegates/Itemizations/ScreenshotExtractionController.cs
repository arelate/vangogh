using System.Collections.Generic;
using System.Text.RegularExpressions;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeScreenshotsDelegate : IItemizeDelegate<string, string>
    {
        private const string attributePrefix = "data-src=\"";
        private readonly Regex regex = new Regex(attributePrefix + "\\S*\"");

        public IEnumerable<string> Itemize(string pageContent)
        {
            var match = regex.Match(pageContent);
            while (match.Success)
            {
                var screenshot = match.Value.Substring(attributePrefix.Length, // drop the prefix data-src="
                    match.Value.Length - attributePrefix.Length - 1); // and closing "

                yield return screenshot;
                match = match.NextMatch();
            }
        }
    }
}