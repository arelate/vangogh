using Interfaces.Delegates.Replace;

namespace Delegates.Replace
{
    public class ReplaceMultipleStringsDelegate : IReplaceMultipleDelegate<string>
    {
        public string ReplaceMultiple(string input, string replaceWith, params string[] findWhat)
        {
            foreach (var match in findWhat)
                input = input.Replace(match, replaceWith);

            return input;
        }
    }
}
