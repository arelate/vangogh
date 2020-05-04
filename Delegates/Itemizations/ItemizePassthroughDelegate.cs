using System.Collections.Generic;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations
{
    public class ItemizePassthroughDelegate : IItemizeDelegate<string, string>
    {
        public IEnumerable<string> Itemize(string item)
        {
            return new string[] {item};
        }
    }
}