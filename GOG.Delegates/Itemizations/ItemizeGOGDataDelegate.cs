using System.Collections.Generic;
using System.Text.RegularExpressions;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeGOGDataDelegate : IItemizeDelegate<string, string>
    {
        private const string gogDataPrefix = "var gogData = ";
        private readonly Regex regex = new Regex(gogDataPrefix + "(.*)");

        public IEnumerable<string> Itemize(string data)
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