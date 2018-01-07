using System.Collections.Generic;

namespace Interfaces.Delegates.BreakLines
{
    // TODO: isn't this Constrain delegate?
    public interface IBreakLinesDelegate
    {
        IEnumerable<string> BreakLines(int width, IEnumerable<string> lines);
    }
}
