using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.LineBreaking
{
    public interface ILineBreakingDelegate
    {
        IEnumerable<string> BreakLines(int width, IEnumerable<string> lines);
    }
}
