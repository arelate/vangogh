using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Interfaces.Extraction;

namespace Controllers.Extraction
{
    public class GameDetailsLanguagesExtractionController : IStringExtractionController
    {
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
    }
}
