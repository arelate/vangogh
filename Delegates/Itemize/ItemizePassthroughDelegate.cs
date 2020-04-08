using System.Collections.Generic;
using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize
{
    public class ItemizePassthroughDelegate : IItemizeDelegate<string, string>
    {
        public IEnumerable<string> Itemize(string item)
        {
            return new string[] {item};
        }
    }
}