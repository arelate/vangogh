using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Interfaces.Extraction;

namespace Controllers.Extraction
{
    public class ScreenshotExtractionController : IStringExtractionController
    {
        private const string attributePrefix = "data-src=\"";
        private Regex regex = new Regex(attributePrefix + "\\S*\"");

        public IEnumerable<string> ExtractMultiple(string pageContent)
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
