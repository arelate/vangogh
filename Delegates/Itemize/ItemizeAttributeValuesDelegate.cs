using System.Collections.Generic;
using System.Text.RegularExpressions;

using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize
{
    public abstract class ItemizeAttributeValuesDelegate: IItemizeDelegate<string, string>
    {
        readonly string pattern;

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
                string valueAttribute = "value=\"";
                var start = match.Value.IndexOf(valueAttribute, System.StringComparison.Ordinal) + valueAttribute.Length;
                yield return match.Value.Substring(start, match.Value.Length - start - 1);

                match = match.NextMatch();
            }
        }
    }
}
