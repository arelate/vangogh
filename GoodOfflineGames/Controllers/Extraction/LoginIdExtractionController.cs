using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Extraction;

namespace Controllers.Extraction
{
    public class LoginIdExtractionController: IExtractionController
    {
        public IEnumerable<string> ExtractMultiple(string data)
        {
            // extracting login id value
            const string loginIdValuePattern = @"\d{17}";
            var regex = new Regex(loginIdValuePattern);
            var match = regex.Match(data);
            while (match.Success)
            {
                yield return match.Value;
                match = match.NextMatch();
            }
        }
    }
}
