using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Extraction;

namespace Controllers.Extraction
{
    public abstract class ValueAttributeExtractionController: IExtractionController
    {
        internal string pattern;

        internal virtual string ExtractValue(string data)
        {
            string valueAttribute = "value=\"";
            var start = data.IndexOf(valueAttribute) + valueAttribute.Length;
            return data.Substring(start, data.Length - start  - 1);
        }

        public virtual IEnumerable<string> ExtractMultiple(string data)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(data);
            while (match.Success)
            {
                yield return ExtractValue(match.Value);
                match = match.NextMatch();
            }
        }
    }
}
