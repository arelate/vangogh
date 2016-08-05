using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Extraction;

namespace Controllers.Extraction
{
    // TODO: add tests
    public class ExtractionController : IExtractionController
    {
        public IEnumerable<string> ExtractMultiple(string data)
        {
            // extracting login token that is 43 characters (letters, numbers, - ...)
            const string tokenPattern = @"[\w-]{43}";
            var regex = new Regex(tokenPattern);
            var match = regex.Match(data);
            while (match.Success)
            {
                yield return match.Value;
                match = match.NextMatch();
            }
        }
    }
}
