using System.Collections.Generic;
using System.Text.RegularExpressions;

using GOG.Interfaces;

namespace GOG.Controllers
{
    // TODO: add tests
    public class TokenExtractorController : ITokenExtractorController
    {
        public IEnumerable<string> ExtractMultiple(string data)
        {
            // extracting login token that is 43 characters (letters, numbers, - ...)
            Regex regex = new Regex(@"[\w-]{43}");
            var match = regex.Match(data);
            while (match.Success)
            {
                yield return match.Value;
                match = match.NextMatch();
            }
        }
    }
}
