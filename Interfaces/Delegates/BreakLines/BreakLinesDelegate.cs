using System.Collections.Generic;

namespace Interfaces.Delegates.BreakLines
{
    public interface IBreakLinesDelegate
    {
        IEnumerable<string> BreakLines(int width, IEnumerable<string> lines);
    }
}
