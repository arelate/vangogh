using System.Collections.Generic;
using System.Text.RegularExpressions;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations
{
    public abstract class ItemizeAttributeValuesDelegate : IItemizeDelegate<string, string>
    {
        private readonly string pattern;

        public ItemizeAttributeValuesDelegate(string pattern)
        {
            this.pattern = pattern;
        }

        public IEnumerable<string> Itemize(string data)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(data);
            while (match.Success)
            {
                var valueAttribute = "value=\"";
                var start = match.Value.IndexOf(valueAttribute, System.StringComparison.Ordinal) +
                            valueAttribute.Length;
                yield return match.Value.Substring(start, match.Value.Length - start - 1);

                match = match.NextMatch();
            }
        }
    }
}