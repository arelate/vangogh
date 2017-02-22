using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Extraction;

namespace Controllers.Extraction
{
    public class GOGDataExtractionController : IStringExtractionController
    {
        private const string gogDataPrefix = "var gogData = ";
        private Regex regex = new Regex(gogDataPrefix + "(.*)");

        public IEnumerable<string> ExtractMultiple(string data)
        {
            var match = regex.Match(data);
            var gogData = new List<string>();

            while (match.Success)
            {
                var gogDataString = match.Value.Substring(
                    gogDataPrefix.Length, // drop the prefix var gogData = 
                    match.Value.Length - gogDataPrefix.Length - 1); // and closing ";"
                gogData.Add(gogDataString);

                match = match.NextMatch();
            }

            return gogData;
        }
    }
}
